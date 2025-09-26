using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private GameState _state;
    private int currentMenuButton;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private GameObject attackLabelField;
    [SerializeField] private TMP_Text attackLabel;
    [SerializeField] private MenuButton[] menuButtons;
    [SerializeField] private Image portrait;
    [SerializeField] private List<CharacterData> playerUnits;
    [SerializeField] private List<CharacterData> enemyUnits;
    [SerializeField] private BattleCharObject battleCharPrefab;
    [SerializeField] private GameObject playerObjPanel;
    [SerializeField] private GameObject enemyObjPanel;
    [SerializeField] private List<BattleCharObject> playerBattleObjects;
    [SerializeField] private List<BattleCharObject> enemyBattleObjects;

    private void Start()
    {
        playerUnits = GameManager.instance.party;
        enemyUnits = GameManager.instance.GetActiveEnemyList();
        staminaSlider.maxValue = GameManager.instance.maxStamina;
        AdjustStamina(0);
        attackField.SetActive(false);
        subMenuField.SetActive(false);
        magicAttackField.SetActive(false);
        attackLabelField.SetActive(false);
        foreach (var button in menuButtons)
            button.DeselectButton();
        menuButtons[currentMenuButton].SelectButton();
        
        foreach (CharacterData unit in enemyUnits)
            unit.InitialiseChar();

        playerStartPos = player.transform.position;
        currentActiveTeam = playerBattleObjects;
        CreateAttackScene();
        if (CheckDeadTeam(playerBattleObjects) == false)
            LostBattle();
        else if (CheckDeadTeam(enemyBattleObjects) == false)
            WonBattle();
        SetActiveAttacker();
    }

    private void CreateAttackScene()
    {
        CreateCharObjectDisplay(playerObjPanel, playerUnits, playerBattleObjects);
        CreateCharObjectDisplay(enemyObjPanel, enemyUnits, enemyBattleObjects);
    }

    private void CreateCharObjectDisplay(GameObject panel, List<CharacterData> unitList, List<BattleCharObject> objList)
    {
        foreach (BattleCharObject c in objList)
        {
            Destroy(c.gameObject);
        }
        objList.Clear();
        float panelPos = panel.transform.position.y + (float)6.3 / 2;
        float panelStep = (float)6.3/unitList.Count;
        float step = 0.5f;
        foreach (CharacterData unit in unitList)
        {
            Vector2 panelPosition = new Vector2(panel.transform.position.x, panelPos + -(panelStep*step));
            BattleCharObject newCharObject = Instantiate(battleCharPrefab, panelPosition, panel.transform.rotation, panel.transform);
            newCharObject.transform.Translate(transform.right * -step/2);
            newCharObject.SetCharacterObject(unit, (int)step);
            objList.Add(newCharObject);
            step++;
        }
    }

    public void SwitchCharacterTeam(CharacterData unit)
    {
        StopAllCoroutines();
        if (playerUnits.Contains(unit))
        {
            playerUnits.Remove(unit);
            enemyUnits.Add(unit);
        }
        else
        {
            enemyUnits.Remove(unit);
            playerUnits.Add(unit);
        }
        CreateAttackScene();
    }

    private void Update()
    {
        if (_state == GameState.AttackMenu)
        {
            MenuControl();
        }
        else if (_state == GameState.PlayerAttack)
        {
            PhysicalAttackControl();
        }
        else if (_state == GameState.SubMenu)
        {
            SubMenuControl();
        }
        else if (_state == GameState.SelectingTargets)
        {
            SelectTargetControl();
        }
        else if (_state == GameState.MagicAttack)
        {
            MagicAttackControl();
        }
    }

    private bool enemyTurn;
    private List<BattleCharObject> currentActiveTeam;
    private int battleIndex;
    private void SetNewAttacker()
    {
        currentActiveTeam[battleIndex].TickConditions();

        if (CheckDeadTeam(playerBattleObjects) == false)
            LostBattle();
        else if (CheckDeadTeam(enemyBattleObjects) == false)
            WonBattle();

        
        battleIndex++;
        if (battleIndex >= currentActiveTeam.Count)
        {
            battleIndex = 0;
            enemyTurn = !enemyTurn;
            if (enemyTurn)
            {
                currentActiveTeam = enemyBattleObjects;
                BeginMagicPhase(true);
            }
            else
            {
                currentActiveTeam = playerBattleObjects;
                BeginMagicPhase(false);
            }   
        }
        else
        {
            SetActiveAttacker();
        }
    }

    private void SetActiveAttacker()
    {
        if (currentActiveTeam[battleIndex].GetCharacter().downed)
        {
            SetNewAttacker();
        }
        else
        {
            if (!enemyTurn)
            {
                _state = GameState.AttackMenu;
                if (playerUnits[battleIndex].unit is Unit_Player)
                {
                    Unit_Player character = (Unit_Player)playerUnits[battleIndex].unit;
                    portrait.sprite = character.charPortrait;
                }
                else
                {
                    portrait.sprite = playerUnits[battleIndex].unit.battleSprite;
                }
            }
            else
            {
                BeginEnemyAttack();
            }
        }
    }

    private void AdjustStamina(int i)
    {
        if (i != 0)
            GameManager.instance.AdjustStamina(i);
        staminaSlider.value = GameManager.instance.stamina;
        staminaText.text = GameManager.instance.stamina.ToString() + "/" + GameManager.instance.maxStamina.ToString();
    }

    private void ShowAttackLabel(string inputText, string targetName)
    {
        attackLabelField.SetActive(true);
        attackLabel.text = inputText + ": " + targetName;
    }

    private void MenuControl()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchMenuButton(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchMenuButton(1);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            menuButtons[currentMenuButton].PressButton();
        }
    }

    public void SwitchMenuButton(int dir)
    {
        menuButtons[currentMenuButton].DeselectButton();
        currentMenuButton += dir;
        if (currentMenuButton < 0)
            currentMenuButton = menuButtons.Length-1;
        else if (currentMenuButton >= menuButtons.Length)
            currentMenuButton = 0;
        menuButtons[currentMenuButton].SelectButton();
    }

    public void SelectAttackButton()
    {
        PlayerSelectTargets(playerUnits[battleIndex].weapon);
    }

    public void SelectSkillButton()
    {
        if (playerUnits[battleIndex].skills.Count != 0)
        {
            OpenSubMenu(playerUnits[battleIndex].GetSkills());
        }
    }

    public void SelectGuardButton(Skill skill)
    {
        PlayerSelectTargets(skill);
    }

    public void SelectItemsButton()
    {
        OpenSubMenu(GameManager.instance.GetItemList());
    }

    [SerializeField] private List<BattleCharObject> targets;
    private int currentSelectTarget;
    private bool selectSpecificTarget;
    private List<BattleCharObject> targetArray;
    private Ability currentAbility;
    private bool selectDowned;
    private void PlayerSelectTargets(Ability ability)
    {
        targets.Clear();
        _state = GameState.SelectingTargets;
        currentAbility = ability;
        currentSelectTarget = 0;
        selectSpecificTarget = false;
        switch (ability.GetAction().targetingType)
        {
            case TargetingType.SingleEnemy:
                foreach (BattleCharObject i in enemyBattleObjects)
                {
                    i.SetDeselected();
                }
                selectSpecificTarget = true;
                selectDowned = false;
                targetArray = enemyBattleObjects;
                int breakThreshold = 0;
                while (targetArray[currentSelectTarget].GetCharacter().downed)
                {
                    currentSelectTarget++;
                    if (currentSelectTarget >= targetArray.Count)
                        currentSelectTarget = 0;
                    breakThreshold++;
                    if (breakThreshold > 10)
                        break;
                }
                targetArray[currentSelectTarget].SetYellow();
                targets.Add(enemyBattleObjects[currentSelectTarget]);
                ShowAttackLabel(ability.name, targets[0].GetCharacter().unit.name);
                break;
            case TargetingType.AllEnemies:
                ShowAttackLabel(ability.name, "All");
                foreach (BattleCharObject i in enemyBattleObjects)
                {
                    i.SetYellow();
                    targets.Add(i);
                }
                targetArray = enemyBattleObjects;
                break;
            case TargetingType.SingleAlly:
                foreach (BattleCharObject i in playerBattleObjects)
                {
                    i.SetDeselected();
                }
                selectSpecificTarget = true;
                selectDowned = true;
                targetArray = playerBattleObjects;
                playerBattleObjects[currentSelectTarget].SetYellow();
                targets.Add(playerBattleObjects[currentSelectTarget]);
                ShowAttackLabel(ability.name, targets[0].GetCharacter().unit.name);
                break;
            case TargetingType.AllAllies:
                foreach (BattleCharObject i in playerBattleObjects)
                {
                    i.SetYellow();
                    targets.Add(i);
                }
                targetArray = playerBattleObjects;
                ShowAttackLabel(ability.name, "Allies");
                break;
            case TargetingType.Self:
                foreach (BattleCharObject i in playerBattleObjects)
                {
                    i.SetDeselected();
                }
                currentSelectTarget = battleIndex;
                targetArray = playerBattleObjects;
                playerBattleObjects[currentSelectTarget].SetYellow();
                targets.Add(playerBattleObjects[currentSelectTarget]);
                ShowAttackLabel(ability.name, "Self");
                break;
            default:
                Debug.LogError("Error Getting Action Target");
                break;
        }
    }
    private void SelectTargetControl()
    {
        _state = GameState.SelectingTargets;
        if (selectSpecificTarget)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetArray[currentSelectTarget].SetDeselected();
                targets.Remove(targetArray[currentSelectTarget]);
                currentSelectTarget++;
                if (currentSelectTarget >= targetArray.Count)
                    currentSelectTarget = 0;
                if (!selectDowned)
                {
                    int breakThreshold = 0;
                    while (targetArray[currentSelectTarget].GetCharacter().downed)
                    {
                        currentSelectTarget++;
                        if (currentSelectTarget >= targetArray.Count)
                            currentSelectTarget = 0;
                        breakThreshold++;
                        if (breakThreshold > 10)
                            break;
                    }
                }
                targets.Add(targetArray[currentSelectTarget]);
                ShowAttackLabel(currentAbility.name, targets[0].GetCharacter().unit.name);
                targetArray[currentSelectTarget].SetYellow();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetArray[currentSelectTarget].SetDeselected();
                targets.Remove(targetArray[currentSelectTarget]);
                currentSelectTarget--;
                if (currentSelectTarget < 0)
                    currentSelectTarget = targetArray.Count - 1;
                if (!selectDowned)
                {
                    int breakThreshold = 0;
                    while (targetArray[currentSelectTarget].GetCharacter().downed)
                    {
                        currentSelectTarget++;
                        if (currentSelectTarget >= targetArray.Count)
                            currentSelectTarget = 0;
                        breakThreshold++;
                        if (breakThreshold > 10)
                            break;
                    }
                }
                targets.Add(targetArray[currentSelectTarget]);
                ShowAttackLabel(currentAbility.name, targets[0].GetCharacter().unit.name);
                targetArray[currentSelectTarget].SetYellow();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (BattleCharObject i in targetArray)
            {
                i.SetWhite();
            }
            switch (currentAbility)
            {
                case Item_Weapon:
                    PlayerPhysicalAttack(currentAbility.GetAction());
                    break;
                case Item_Usable:
                    Item_Usable item = currentAbility as Item_Usable;
                    if (item.action.targetingType == TargetingType.AllEnemies || item.action.targetingType == TargetingType.SingleEnemy)
                    {
                        PlayerPhysicalAttack(currentAbility.GetAction());
                    }
                    else
                    {
                        if (currentAbility.GetAction().staminaAdjust != 0)
                            AdjustStamina(currentAbility.GetAction().staminaAdjust);
                        ApplyAction(currentAbility.GetAction().damageStats);
                    }
                    GameManager.instance.UseItem(currentAbility as Item_Usable);
                    break;
                case Skill_Magic:
                    Skill_Magic spell = currentAbility as Skill_Magic;
                    if (spell.action.targetingType == TargetingType.AllEnemies || spell.action.targetingType == TargetingType.SingleEnemy)
                    {
                        /*magicPatterns = spell.magicPattern;
                        castingSpeed = spell.castingSpeed;
                        mageTargets = targets;
                        SetNewAttacker();*/
                        AttackHit(currentAbility.GetAction().damageStats, false);
                        SetNewAttacker();
                    }
                    else
                    {
                        ApplyAction(currentAbility.GetAction().damageStats);
                    }
                    AdjustStamina(-spell.staminaCost);
                    break;
                case Skill:
                    Skill skill = currentAbility as Skill;
                    if (skill.action.targetingType == TargetingType.AllEnemies || skill.action.targetingType == TargetingType.SingleEnemy)
                    {
                        PlayerPhysicalAttack(currentAbility.GetAction());
                    }
                    else
                    {
                        ApplyAction(currentAbility.GetAction().damageStats);
                    }
                    AdjustStamina(-skill.staminaCost);
                    break;
            }
            

            attackLabelField.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            foreach(BattleCharObject i in targetArray)
            {
                i.SetWhite();
            }
            _state = GameState.AttackMenu;

            attackLabelField.SetActive(false);
        }
    }

    [Header("PhysicalAttackSettings")]
    [SerializeField] private GameObject attackField;
    [SerializeField] private Slider attackSlider;
    [SerializeField] private RectTransform critChanceObject;
    [SerializeField] private RectTransform hitChanceObject;
    [SerializeField] private RectTransform sliderPos;
    private float sliderValue;
    [SerializeField] private float sliderSpeed;
    

    private void PlayerPhysicalAttack(Action action)
    {
        attackField.SetActive(true);
        sliderValue = 0;
        float accuracyBarLength = action.accuracy * playerUnits[battleIndex].currentStats.accuracy;


         if (action.useCrit)
         {
             critChanceObject.gameObject.SetActive(true);
             critChanceObject.localPosition = new Vector2(Random.Range(35f, 80f), 0);
             critChanceObject.sizeDelta = new Vector2((float)accuracyBarLength * playerUnits[battleIndex].currentStats.critPercent, 20);

             hitChanceObject.localPosition = new Vector2(critChanceObject.localPosition.x - (critChanceObject.sizeDelta.x * 0.5f), 0);
             hitChanceObject.sizeDelta = new Vector2((float)accuracyBarLength, 20);
         }
         else
         {
             critChanceObject.gameObject.SetActive(false);
             hitChanceObject.localPosition = new Vector2(Random.Range(35, 80), 0);
             hitChanceObject.sizeDelta = new Vector2((float)accuracyBarLength, 20);
         }
        _state = GameState.PlayerAttack;
    }
    private void PhysicalAttackControl()
    {
        if (Input.GetKeyDown(KeyCode.Z) || attackSlider.value == 1)
        {
            if (currentAbility.GetAction().useCrit && sliderPos.localPosition.x >= critChanceObject.localPosition.x - (critChanceObject.sizeDelta.x) && sliderPos.localPosition.x <= (critChanceObject.localPosition.x + 5))
            {
                AttackHit(currentAbility.GetAction().damageStats, true);
            }
            else if (sliderPos.localPosition.x >= hitChanceObject.localPosition.x - (hitChanceObject.sizeDelta.x + 5) && sliderPos.localPosition.x <= (hitChanceObject.localPosition.x + 5))
            {
                AttackHit(currentAbility.GetAction().damageStats, false);
            }
            else
            {
                AttackMiss();
            }
            attackField.SetActive(false);
            SetNewAttacker();
        }

        sliderValue += sliderSpeed * Time.deltaTime;
        attackSlider.value = sliderValue;
    }

    private void AttackHit(DamageStats damageStats, bool crit)
    {
        DamageStats damage = new DamageStats(damageStats);
        if (crit)
            damage.damage = (int)(damage.damage * currentActiveTeam[battleIndex].GetCharacter().currentStats.attack * currentActiveTeam[battleIndex].GetCharacter().currentStats.critMultiplyer);
        else
            damage.damage = (int)(damage.damage * currentActiveTeam[battleIndex].GetCharacter().currentStats.attack);


        StartCoroutine(currentActiveTeam[battleIndex].SetSprite(0, 0.75f, currentAbility.GetAction().spriteIndex));
        foreach (BattleCharObject i in targets)
        {
            StartCoroutine(i.SetSprite(1, 0.75f, 0));
            i.TakeDamage(damage, crit);
        }
    }

    private void AttackMiss()
    {
        StartCoroutine(currentActiveTeam[battleIndex].SetSprite(0, 0.75f, currentAbility.GetAction().spriteIndex));
        foreach (BattleCharObject i in targets)
        {
            i.AttackMissed();
        }
    }

    private void ApplyAction(DamageStats damageStats)
    {
        foreach (BattleCharObject i in targets)
        {
            i.TakeDamage(damageStats, false);
        }
        SetNewAttacker();
    }

    [Header("MagicAttackSettings")]
    [SerializeField] private GameObject magicAttackField;
    [SerializeField] private Rigidbody2D player;
    private Vector2 playerStartPos;
    [SerializeField] private GameObject caster;
    [SerializeField] private BaseShooter castingObj;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float casterMoveSpeed;
    private bool casting;
    private PatternSettings magicPatterns;
    public List<BattleCharObject> mageTargets;
    private float castingSpeed;
    private float timeBtwCast;
    private bool canCast = true;
    private float enemyCastRotMult = 1f;
    private void BeginMagicPhase(bool playerCasting)
    {
        player.transform.position = playerStartPos;
        if (magicPatterns != null)
        {
            _state = GameState.MagicAttack;
            magicAttackField.SetActive(true);
            castingObj.pattern = magicPatterns;
            casting = playerCasting;
            if (!casting)
                StartCoroutine(RandEnemyCasterMovement(Random.Range(0.01f, 0.65f)));
            timeBtwCast = 15;
            canCast = true;
            StartCoroutine(MagicAttackTimer(6f));
        }
        else
        {
            SetActiveAttacker();
        }
    }
    private void MagicAttackControl()
    {
        if (timeBtwCast <= 0)
        {
            canCast = true;
        }
        else
        {
            timeBtwCast -= 1 * Time.deltaTime;
        }

        if (casting)
        {
            float movement = 0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow))
                movement = -1;

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow))
                movement = 1;

            if (canCast && Input.GetKeyDown(KeyCode.Z))
            {
                castingObj.ActivatePatternSpawner();
                canCast = false;
                timeBtwCast = castingSpeed;
            }

            caster.transform.rotation *= Quaternion.Euler(0, 0, movement * casterMoveSpeed * Time.deltaTime);
        }
        else
        {
            Vector2 movement = new Vector2(0f, 0f);
            if (Input.GetKey(KeyCode.LeftArrow))
                movement += new Vector2(-1, 0);

            if (Input.GetKey(KeyCode.RightArrow))
                movement += new Vector2(1, 0);

            if (Input.GetKey(KeyCode.UpArrow))
                movement += new Vector2(0, 1);

            if (Input.GetKey(KeyCode.DownArrow))
                movement += new Vector2(0, -1);

            movement.Normalize();
            player.AddForce(movement * Time.deltaTime * playerMoveSpeed, ForceMode2D.Impulse);

            //EnemyCaster
            caster.transform.rotation *= Quaternion.Euler(0, 0, 0.5f * casterMoveSpeed * enemyCastRotMult * Time.deltaTime);
            if (canCast)
            {
                castingObj.ActivatePatternSpawner();
                canCast = false;
                timeBtwCast = castingSpeed;
            }
        }
    }

    private IEnumerator RandEnemyCasterMovement(float timer)
    {
        int reverse = Random.Range(0, 2);
        if (reverse == 0)
            enemyCastRotMult = -1;
        else
            enemyCastRotMult = 1;
        yield return new WaitForSeconds(timer);
        StartCoroutine(RandEnemyCasterMovement(Random.Range(0.01f, 0.65f)));
    }

    private IEnumerator MagicAttackTimer(float time)
    {
        attackLabelField.SetActive(false);
        yield return new WaitForSeconds(time);
        SetActiveAttacker();
        StopAllCoroutines();
        magicPatterns = null;
        magicAttackField.SetActive(false);
        ObjectPool._instance.SetBulletsInactive();
    }

    public void MagicAttackHit(DamageStats damageStats)
    {
        foreach (BattleCharObject i in mageTargets)
        {
            //StartCoroutine(i.SetSprite(1, 0.75f));
            i.TakeDamage(damageStats, false);
        }
    }

    [Header("AbilitySettings")]
    [SerializeField] private GameObject subMenuField;
    [SerializeField] private GameObject subMenuArea;
    [SerializeField] private GameObject subMenuItemPrefab;
    [SerializeField] private GameObject descriptionBox;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    private List<SubMenuButton> subMenuItems = new List<SubMenuButton>();
    private int currentSubButton;
    public void OpenSubMenu(List<Ability> abilityList)
    {
        subMenuField.SetActive(true);
        foreach (Transform oldSkill in subMenuArea.transform)
        {
            subMenuItems.Clear();
            Destroy(oldSkill.gameObject);
        }

        foreach (Ability abilityItem in abilityList)
        {
            SubMenuButton newItem = Instantiate(subMenuItemPrefab, subMenuArea.transform.position, Quaternion.identity, subMenuArea.transform).GetComponent<SubMenuButton>();
            newItem.SetSkill(abilityItem);
            subMenuItems.Add(newItem);
            newItem.DeselectButton();
        }
        currentSubButton = 0;
        subMenuItems[currentSubButton].SelectButton();
        descriptionBox.transform.position = new Vector2(descriptionBox.transform.position.x, subMenuItems[currentSubButton].gameObject.transform.position.y);
        titleText.text = "Selected:\n" + subMenuItems[currentSubButton].GetSkill().name;
        descriptionText.text = subMenuItems[currentSubButton].GetSkill().description;
        _state = GameState.SubMenu;
    }
    private void SubMenuControl()
    {
        if (subMenuItems.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                subMenuItems[currentSubButton].DeselectButton();
                currentSubButton++;
                if (currentSubButton >= subMenuItems.Count)
                    currentSubButton = 0;
                subMenuItems[currentSubButton].SelectButton();
                descriptionBox.transform.position = new Vector2(descriptionBox.transform.position.x, subMenuItems[currentSubButton].gameObject.transform.position.y);
                titleText.text = "Selected:\n" + subMenuItems[currentSubButton].GetSkill().name;
                descriptionText.text = subMenuItems[currentSubButton].GetSkill().description;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                subMenuItems[currentSubButton].DeselectButton();
                currentSubButton--;
                if (currentSubButton < 0)
                    currentSubButton = subMenuItems.Count - 1;
                subMenuItems[currentSubButton].SelectButton();
                descriptionBox.transform.position = new Vector2(descriptionBox.transform.position.x, subMenuItems[currentSubButton].gameObject.transform.position.y);
                titleText.text = "Selected:\n" + subMenuItems[currentSubButton].GetSkill().name;
                descriptionText.text = subMenuItems[currentSubButton].GetSkill().description;
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Ability selectedAbility = subMenuItems[currentSubButton].GetSkill();
                CloseSubMenu();
                if (selectedAbility is Skill)
                {
                    Skill skill = selectedAbility as Skill;
                    if (GameManager.instance.stamina >= skill.staminaCost)
                    {
                        PlayerSelectTargets(skill);
                    }
                }
                else if (selectedAbility is Item_Usable)
                {
                    PlayerSelectTargets(selectedAbility as Item_Usable);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseSubMenu();
        }
    }

    private void CloseSubMenu()
    {
        subMenuField.SetActive(false);
        _state = GameState.AttackMenu;
    }


    private void BeginEnemyAttack()
    {
        _state = GameState.EnemyAttack;
        int useSkill = Random.Range(0, 5);
        if (useSkill > 2)
        {
            List<Ability> enemySkills = currentActiveTeam[battleIndex].GetCharacter().GetSkills();
            currentAbility = enemySkills[Random.Range(0, enemySkills.Count)];
        }
        else
        {
            currentAbility = currentActiveTeam[battleIndex].GetCharacter().weapon;
        }
        StartCoroutine(EnemyAttack());
    }

    private List<BattleCharObject> EnemyChooseTargets(TargetingType targeting)
    {
        List<BattleCharObject> enemyTargets = new List<BattleCharObject>();
        switch (targeting)
        {
            case TargetingType.SingleEnemy:
                int targetIndex = Random.Range(0, playerBattleObjects.Count);
                BattleCharObject selectedObj = playerBattleObjects[targetIndex];
                int breakThreshold = 0;
                while (selectedObj.GetCharacter().downed)
                {
                    targetIndex++;
                    if (targetIndex >= playerBattleObjects.Count)
                        targetIndex = 0;
                    selectedObj = playerBattleObjects[targetIndex];
                    breakThreshold++;
                    if (breakThreshold > 10)
                        break;
                }
                enemyTargets.Add(selectedObj);
                ShowAttackLabel(currentAbility.name, selectedObj.GetCharacter().unit.name);
                break;
            case TargetingType.AllEnemies:
                enemyTargets = playerBattleObjects;
                ShowAttackLabel(currentAbility.name, "All");
                break;
            case TargetingType.SingleAlly:
                enemyTargets.Add(enemyBattleObjects[Random.Range(0, enemyBattleObjects.Count)]);
                ShowAttackLabel(currentAbility.name, enemyTargets[0].GetCharacter().unit.name);
                break;
            case TargetingType.AllAllies:
                enemyTargets = enemyBattleObjects;
                ShowAttackLabel(currentAbility.name, "Allies");
                break;
            case TargetingType.Self:
                enemyTargets.Add(currentActiveTeam[battleIndex]);
                ShowAttackLabel(currentAbility.name, "Self");
                break;
        }
        return enemyTargets;
    }

    private IEnumerator EnemyAttack()
    {
        targets.Clear();
        targets.AddRange(EnemyChooseTargets(currentAbility.GetAction().targetingType));
        yield return new WaitForSeconds(1f);

        switch (currentAbility)
        {
            case Item_Weapon:
                EnemyAttemptHit();
                yield return new WaitForSeconds(0.4f);
                attackLabelField.SetActive(false);
                SetNewAttacker();
                break;
            case Skill_Magic:
                Skill_Magic spell = currentAbility as Skill_Magic;
                if (spell.action.targetingType == TargetingType.AllEnemies || spell.action.targetingType == TargetingType.SingleEnemy)
                {
                    magicPatterns = spell.magicPattern;
                    castingSpeed = spell.castingSpeed;
                    mageTargets = EnemyChooseTargets(currentAbility.GetAction().targetingType);
                    yield return new WaitForSeconds(0.4f);
                    attackLabelField.SetActive(false);
                    SetNewAttacker();
                }
                else
                {
                    yield return new WaitForSeconds(0.4f);
                    attackLabelField.SetActive(false);
                    ApplyAction(currentAbility.GetAction().damageStats);
                }
                break;
            case Skill:
                Skill skill = currentAbility as Skill;
                if (skill.action.targetingType == TargetingType.AllEnemies || skill.action.targetingType == TargetingType.SingleEnemy)
                {
                    EnemyAttemptHit();
                    yield return new WaitForSeconds(0.4f);
                    attackLabelField.SetActive(false);
                    SetNewAttacker();
                }
                else
                {
                    yield return new WaitForSeconds(0.4f);
                    attackLabelField.SetActive(false);
                    ApplyAction(currentAbility.GetAction().damageStats);
                }
                break;
        }
    }

    private void EnemyAttemptHit()
    {
        ShowAttackLabel(currentAbility.name, targets[0].GetCharacter().unit.name);
        int hitChance = Random.Range(0, 100);
        if (currentAbility.GetAction().useCrit && hitChance <= currentActiveTeam[battleIndex].GetCharacter().currentStats.accuracy * currentAbility.GetAction().accuracy * currentActiveTeam[battleIndex].GetCharacter().currentStats.critPercent)
        {
            AttackHit(currentAbility.GetAction().damageStats, true);
        }
        else if (hitChance <= currentActiveTeam[battleIndex].GetCharacter().currentStats.accuracy * currentAbility.GetAction().accuracy)
        {
            AttackHit(currentAbility.GetAction().damageStats, false);
        }
        else
        {
            AttackMiss();
        }
    }

    private bool CheckDeadTeam(List<BattleCharObject> team)
    {
        bool liveUnits = false;
        if (team.Count <= 0)
            return false;
        foreach (BattleCharObject target in team)
            if (target.GetCharacter().downed == false)
                liveUnits = true;
        return liveUnits;
    }
    private void WonBattle()
    {
        GameManager.instance.WonBattle();
    }

    private void LostBattle()
    {
        GameManager.instance.LoadMenu();
    }
}

public enum GameState
{
    AttackMenu,
    SubMenu,
    SelectingTargets,
    PlayerAttack,
    EnemyAttack,
    MagicAttack
}

public enum TargetingType
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Self
}

