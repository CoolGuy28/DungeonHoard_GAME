using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [SerializeField] private List<CharacterData> randomEnemies;
    [SerializeField] private List<CharacterData> enemyUnits;
    [SerializeField] private BattleCharObject battleCharPrefab;
    [SerializeField] private GameObject playerObjPanel;
    [SerializeField] private GameObject enemyObjPanel;
    [SerializeField] private List<BattleCharObject> playerBattleObjects;
    [SerializeField] private List<BattleCharObject> enemyBattleObjects;

    private void Start()
    {
        playerUnits = GameManager.instance.party;
        enemyUnits.Clear();
        for (int i = 0; i < 3; i++)
            enemyUnits.Add(randomEnemies[Random.Range(0,randomEnemies.Count)]);
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            _state = GameState.MagicAttack;
            casting = true;
            magicAttackField.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
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
            GameManager.instance.stamina += i;
        staminaSlider.value = GameManager.instance.stamina;
        staminaText.text = GameManager.instance.stamina.ToString() + "/" + GameManager.instance.maxStamina.ToString();
    }

    private void ShowAttackLabel(string inputText)
    {
        attackLabelField.SetActive(true);
        attackLabel.text = inputText;
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
    private void PlayerSelectTargets(Ability ability)
    {
        targets.Clear();
        _state = GameState.SelectingTargets;
        currentAbility = ability;
        currentSelectTarget = 0;
        selectSpecificTarget = false;
        ShowAttackLabel(ability.name);
        switch (ability.GetAction().targetingType)
        {
            case TargetingType.SingleEnemy:
                foreach (BattleCharObject i in enemyBattleObjects)
                {
                    i.SetDeselected();
                }
                selectSpecificTarget = true;
                targetArray = enemyBattleObjects;
                enemyBattleObjects[currentSelectTarget].SetYellow();
                targets.Add(enemyBattleObjects[currentSelectTarget]);
                break;
            case TargetingType.AllEnemies:
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
                targetArray = playerBattleObjects;
                playerBattleObjects[currentSelectTarget].SetYellow();
                targets.Add(playerBattleObjects[currentSelectTarget]);
                break;
            case TargetingType.AllAllies:
                foreach (BattleCharObject i in playerBattleObjects)
                {
                    i.SetYellow();
                    targets.Add(i);
                }
                targetArray = playerBattleObjects;
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
                targets.Add(targetArray[currentSelectTarget]);
                targetArray[currentSelectTarget].SetYellow();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetArray[currentSelectTarget].SetDeselected();
                targets.Remove(targetArray[currentSelectTarget]);
                currentSelectTarget--;
                if (currentSelectTarget < 0)
                    currentSelectTarget = targetArray.Count - 1;
                targets.Add(targetArray[currentSelectTarget]);
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
                        ApplyAction(currentAbility.GetAction().damageStats);
                    }
                    GameManager.instance.UseItem(currentAbility as Item_Usable);
                    break;
                case Skill_Magic:
                    Skill_Magic spell = currentAbility as Skill_Magic;
                    if (spell.action.targetingType == TargetingType.AllEnemies || spell.action.targetingType == TargetingType.SingleEnemy)
                    {
                        magicPatterns = spell.magicPattern;
                        castingSpeed = spell.castingSpeed;
                        mageTargets = targets;
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
        critChanceObject.localPosition = new Vector2(Random.Range(35, 80), 0);
        critChanceObject.sizeDelta = new Vector2(action.critChance + (playerUnits[battleIndex].currentStats.accuracy * playerUnits[battleIndex].currentStats.critPercent), 20);

        hitChanceObject.localPosition = new Vector2(critChanceObject.localPosition.x - (critChanceObject.sizeDelta.x * 0.5f), 0);
        hitChanceObject.sizeDelta = new Vector2(action.accuracy + playerUnits[battleIndex].currentStats.accuracy, 20);
        _state = GameState.PlayerAttack;
    }
    private void PhysicalAttackControl()
    {
        if (Input.GetKeyDown(KeyCode.Z) || sliderValue == 100)
        {
            if (sliderPos.position.x >= critChanceObject.position.x - critChanceObject.sizeDelta.x - 5 && sliderPos.position.x <= critChanceObject.position.x + 5)
            {
                DamageStats critStats = new DamageStats(currentAbility.GetAction().damageStats);
                critStats.damage = (int)(critStats.damage * playerUnits[battleIndex].currentStats.critMultiplyer);
                AttackHit(critStats);
            }
            else if (sliderPos.position.x >= hitChanceObject.position.x - hitChanceObject.sizeDelta.x - 5 && sliderPos.position.x <= hitChanceObject.position.x + 5)
            {
                AttackHit(currentAbility.GetAction().damageStats);
            }
            attackField.SetActive(false);
            SetNewAttacker();
        }
        sliderValue += sliderSpeed * Time.deltaTime;
        attackSlider.value = sliderValue;
    }

    private void AttackHit(DamageStats damageStats)
    {
        StartCoroutine(currentActiveTeam[battleIndex].SetSprite(0, 0.75f));
        foreach (BattleCharObject i in targets)
        {
            StartCoroutine(i.SetSprite(1, 0.75f));
            i.TakeDamage(damageStats);
        }
    }

    private void ApplyAction(DamageStats damageStats)
    {
        foreach (BattleCharObject i in targets)
        {
            i.TakeDamage(damageStats);
        }
        SetNewAttacker();
    }

    [Header("MagicAttackSettings")]
    [SerializeField] private GameObject magicAttackField;
    [SerializeField] private GameObject player;
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
    private void BeginMagicPhase(bool playerCasting)
    {
        player.transform.position = playerStartPos;
        if (magicPatterns != null)
        {
            _state = GameState.MagicAttack;
            magicAttackField.SetActive(true);
            castingObj.pattern = magicPatterns;
            casting = playerCasting;
            timeBtwCast = 0;
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
            if (Input.GetKey(KeyCode.LeftArrow) && player.transform.position.x >= (magicAttackField.transform.position.x - magicAttackField.transform.lossyScale.x * 0.5))
                movement += new Vector2(-1, 0);

            if (Input.GetKey(KeyCode.RightArrow) && player.transform.position.x <= (magicAttackField.transform.position.x + magicAttackField.transform.lossyScale.x * 0.5))
                movement += new Vector2(1, 0);

            if (Input.GetKey(KeyCode.UpArrow) && player.transform.position.y <= (magicAttackField.transform.position.y + magicAttackField.transform.lossyScale.y * 0.5))
                movement += new Vector2(0, 1);

            if (Input.GetKey(KeyCode.DownArrow) && player.transform.position.y >= (magicAttackField.transform.position.y - magicAttackField.transform.lossyScale.y * 0.5))
                movement += new Vector2(0, -1);

            movement.Normalize();
            player.transform.Translate(movement * Time.deltaTime * playerMoveSpeed);

            //EnemyCaster
            caster.transform.rotation *= Quaternion.Euler(0, 0, 0.5f * casterMoveSpeed * Time.deltaTime);
            if (canCast)
            {
                castingObj.ActivatePatternSpawner();
                canCast = false;
                timeBtwCast = castingSpeed;
            }
        }
    }

    private IEnumerator MagicAttackTimer(float time)
    {
        yield return new WaitForSeconds(time);
        SetActiveAttacker();
        magicPatterns = null;
        magicAttackField.SetActive(false);
    }

    public void MagicAttackHit(DamageStats damageStats)
    {
        foreach (BattleCharObject i in mageTargets)
        {
            StartCoroutine(i.SetSprite(1, 0.75f));
            i.TakeDamage(damageStats);
        }
    }

    [Header("AbilitySettings")]
    [SerializeField] private GameObject subMenuField;
    [SerializeField] private GameObject subMenuArea;
    [SerializeField] private GameObject subMenuItemPrefab;
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

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                subMenuItems[currentSubButton].DeselectButton();
                currentSubButton--;
                if (currentSubButton < 0)
                    currentSubButton = subMenuItems.Count - 1;
                subMenuItems[currentSubButton].SelectButton();
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
        ShowAttackLabel(currentAbility.name);
        StartCoroutine(EnemyAttack());
    }

    private List<BattleCharObject> EnemyChooseTargets(TargetingType targeting)
    {
        List<BattleCharObject> enemyTargets = new List<BattleCharObject>();
        switch (targeting)
        {
            case TargetingType.SingleEnemy:
                enemyTargets.Add(playerBattleObjects[Random.Range(0, playerBattleObjects.Count)]);
                break;
            case TargetingType.AllEnemies:
                enemyTargets = playerBattleObjects;
                break;
            case TargetingType.SingleAlly:
                enemyTargets.Add(enemyBattleObjects[Random.Range(0, enemyBattleObjects.Count)]);
                break;
            case TargetingType.AllAllies:
                enemyTargets = enemyBattleObjects;
                break;
            case TargetingType.Self:
                enemyTargets.Add(currentActiveTeam[battleIndex]);
                break;
        }
        return enemyTargets;
    }

    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
        targets.Clear();
        targets.AddRange(EnemyChooseTargets(currentAbility.GetAction().targetingType));
        switch (currentAbility)
        {
            case Item_Weapon:
                AttackHit(currentAbility.GetAction().damageStats);
                SetNewAttacker();
                break;
            case Skill_Magic:
                Skill_Magic spell = currentAbility as Skill_Magic;
                if (spell.action.targetingType == TargetingType.AllEnemies || spell.action.targetingType == TargetingType.SingleEnemy)
                {
                    magicPatterns = spell.magicPattern;
                    castingSpeed = spell.castingSpeed;
                    mageTargets = targets;
                    SetNewAttacker();
                }
                else
                {
                    ApplyAction(currentAbility.GetAction().damageStats);
                }
                break;
            case Skill:
                Skill skill = currentAbility as Skill;
                if (skill.action.targetingType == TargetingType.AllEnemies || skill.action.targetingType == TargetingType.SingleEnemy)
                {
                    AttackHit(currentAbility.GetAction().damageStats);
                    SetNewAttacker();
                }
                else
                {
                    ApplyAction(currentAbility.GetAction().damageStats);
                }
                break;
        }
        yield return new WaitForSeconds(0.4f);
        attackLabelField.SetActive(false);
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

