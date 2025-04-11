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
    [SerializeField] private List<CharacterData> enemyUnits;
    private int battleIndex;
    [SerializeField] private BattleCharObject battleCharPrefab;
    [SerializeField] private GameObject playerObjPanel;
    [SerializeField] private GameObject enemyObjPanel;
    [SerializeField] private List<BattleCharObject> playerBattleObjects;
    [SerializeField] private List<BattleCharObject> enemyBattleObjects;
    [SerializeField] private List<BattleCharObject> attackOrder;

    private void Start()
    {
        playerUnits = GameManager.instance.party;
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

        CreateAttackScene();
        SetActiveAttacker();
    }

    private void CreateAttackScene()
    {
        attackOrder.Clear();
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
            attackOrder.Add(newCharObject);
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
            MagicControl();
        }
    }

    private void SetNewAttacker()
    {
        attackOrder[battleIndex].TickConditions();
        battleIndex++;
        if (battleIndex >= attackOrder.Count)
        {
            battleIndex = 0;
            SetActiveAttacker();
        }
        else
        {
            SetActiveAttacker();
        }
    }

    private void SetActiveAttacker()
    {
        if (attackOrder[battleIndex].GetCharacter().downed)
        {
            SetNewAttacker();
        }
        else
        {
            if (playerUnits.Contains(attackOrder[battleIndex].GetCharacter()))
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
                    if (item.attackItem)
                    {
                        PlayerPhysicalAttack(currentAbility.GetAction());
                    }
                    else
                    {
                        PlayerMagicAction();
                    }
                    GameManager.instance.UseItem(currentAbility as Item_Usable);
                    break;
                case Skill:
                    PlayerMagicAction();
                    Skill spell = currentAbility as Skill;
                    AdjustStamina(-spell.staminaCost);
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
        critChanceObject.sizeDelta = new Vector2(action.critChance + playerUnits[battleIndex].currentStats.critChance, 20);

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
                int damage = Random.Range(currentAbility.GetAction().damage * 2 - 10, currentAbility.GetAction().damage * 2 + 10);
                AttackHit(damage, currentAbility.GetAction());
            }
            else if (sliderPos.position.x >= hitChanceObject.position.x - hitChanceObject.sizeDelta.x - 5 && sliderPos.position.x <= hitChanceObject.position.x + 5)
            {
                int damage = Random.Range(currentAbility.GetAction().damage - 10, currentAbility.GetAction().damage + 10);
                AttackHit(damage, currentAbility.GetAction());
            }
            attackField.SetActive(false);
            SetNewAttacker();
        }
        sliderValue += sliderSpeed * Time.deltaTime;
        attackSlider.value = sliderValue;
    }

    private void AttackHit(int damage, Action action)
    {
        StartCoroutine(attackOrder[battleIndex].SetSprite(0, 0.75f));
        foreach (BattleCharObject i in targets)
        {
            if (action.healAction)
            {
                i.HealCharacter(damage, action);
            }
            else
            {
                if (damage <= 0)
                    damage = 0;
                else
                {
                    StartCoroutine(i.SetSprite(1, 0.75f));
                }
                i.TakeDamage((int)(damage * i.GetCharacter().currentStats.defence), action, Color.white);
            }
        }
    }

    private void PlayerMagicAction()
    {
        AttackHit(currentAbility.GetAction().damage, currentAbility.GetAction());
        SetNewAttacker();
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
        //enemyAttackField.SetActive(true);
        //enemyAttacks[Random.Range(0,enemyAttacks.Length)].SetActive(true);
        //StartCoroutine(AttackTimer(5f));
        _state = GameState.EnemyAttack;

        int damage = Random.Range(attackOrder[battleIndex].GetCharacter().weapon.action.damage - 10, attackOrder[battleIndex].GetCharacter().weapon.action.damage + 10);
        ShowAttackLabel(attackOrder[battleIndex].GetCharacter().weapon.name);
        StartCoroutine(EnemyPhysicalAttack(damage, attackOrder[battleIndex].GetCharacter().weapon.action));
    }

    private IEnumerator EnemyPhysicalAttack(int damage, Action action)
    {
        yield return new WaitForSeconds(1f);
        targets.Clear();
        targets.Add(playerBattleObjects[Random.Range(0, playerBattleObjects.Count)]);
        AttackHit(damage, action);
        yield return new WaitForSeconds(0.25f);
        attackLabelField.SetActive(false);
        SetNewAttacker();
    }

    [Header("EnemyAttackSettings")]
    [SerializeField] private GameObject magicAttackField;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject caster;
    [SerializeField] private GameObject catingPoint;
    [SerializeField] private float moveSpeed;
    private void MagicControl()
    {
        /*Vector2 movement = new Vector2(0f, 0f);
        if (Input.GetKey(KeyCode.LeftArrow) && player.transform.position.x >= (magicAttackField.transform.position.x - magicAttackField.transform.lossyScale.x * 0.5))
            movement += new Vector2(-1, 0);

        if (Input.GetKey(KeyCode.RightArrow) && player.transform.position.x <= (magicAttackField.transform.position.x + magicAttackField.transform.lossyScale.x * 0.5))
            movement += new Vector2(1, 0);

        if (Input.GetKey(KeyCode.UpArrow) && player.transform.position.y <= (magicAttackField.transform.position.y + magicAttackField.transform.lossyScale.y * 0.5))
            movement += new Vector2(0, 1);

        if (Input.GetKey(KeyCode.DownArrow) && player.transform.position.y >= (magicAttackField.transform.position.y - magicAttackField.transform.lossyScale.y * 0.5))
            movement += new Vector2(0, -1);

        movement.Normalize();
        player.transform.Translate(movement * Time.deltaTime * moveSpeed);*/

        float movement = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            movement = -1;

        if (Input.GetKey(KeyCode.RightArrow))
            movement = 1;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject bullet = ObjectPool.instance.GetPooledObject();
            bullet.SetActive(true);
            bullet.transform.position = catingPoint.transform.position;
            bullet.transform.rotation = catingPoint.transform.rotation;
            bullet.GetComponent<Projectile>().active = true;
        }

        caster.transform.rotation *= Quaternion.Euler(0, 0, movement * moveSpeed);
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

