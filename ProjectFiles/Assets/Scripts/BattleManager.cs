using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BattleManager : MonoBehaviour
{
    private GameState _state;
    private int currentMenuButton;
    [SerializeField] private MenuButton[] menuButtons;
    [SerializeField] private Image portrait;
    [SerializeField] private List<PlayerUnit> battleInitiative;
    private int initiativeIndex;
    [SerializeField] private Slider enemyHealthUI;
    [SerializeField] private EnemyUnit enemyUnit;
    private int enemyHealth;

    [Header("AttackSettings")]
    [SerializeField] private GameObject attackField;
    [SerializeField] private Slider attackSlider;
    [SerializeField] private GameObject hitChanceObject;
    private Rect hitChance;
    [SerializeField] private GameObject sliderPos;
    private float sliderValue;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private float baseAccuracy;

    [Header("EnemyAttackSettings")]
    [SerializeField] private GameObject enemyAttackField;
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float borderOffset = 0.4f;
    [SerializeField] private GameObject[] enemyAttacks;

    [Header("SkillSettings")]
    [SerializeField] private GameObject skillField;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private GameObject skillArea;


    private void Start()
    {
        enemyHealthUI.maxValue = enemyUnit.health;
        enemyHealth = enemyUnit.health;
        enemyHealthUI.value = enemyHealth;
        attackField.SetActive(false);
        enemyAttackField.SetActive(false);
        foreach (var button in menuButtons)
            button.DeselectButton();
        menuButtons[currentMenuButton].SelectButton();
        SetActiveAttacker();
    }

    private void Update()
    {
        if (_state == GameState.AttackMenu)
        {
            MenuControl();
        }
        else if (_state == GameState.PlayerAttack)
        {
            HitControl();
        }
        else if (_state == GameState.SubMenu)
        {
            SubMenuControl();
        }
    }
    private void FixedUpdate()
    {
        
        if (_state == GameState.EnemyAttack)
        {
            BattleControl();
        }
    }

    public void SetNewAttacker()
    {
        initiativeIndex++;
        if (initiativeIndex >= battleInitiative.Count)
        {
            initiativeIndex = 0;
            BeginEnemyAttack();
        }
        else
        {
            SetActiveAttacker();
        }
    }

    private void SetActiveAttacker()
    {
        _state = GameState.AttackMenu;
        portrait.sprite = battleInitiative[initiativeIndex].charPortrait;
    }

    public void BeginHitAttack()
    {
        attackField.SetActive(true);
        sliderValue = 0;
        hitChanceObject.GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(35, 80), 0);
        hitChanceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(baseAccuracy + battleInitiative[initiativeIndex].accuracy, 20);
        _state = GameState.PlayerAttack;
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

    private void HitControl()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(new GameObject(), new Vector3(hitChanceObject.GetComponent<RectTransform>().position.x - (baseAccuracy + battleInitiative[initiativeIndex].accuracy), 0, 0), Quaternion.identity, hitChanceObject.transform.parent);
            Instantiate(new GameObject(), new Vector3(hitChanceObject.GetComponent<RectTransform>().position.x, 0, 0), Quaternion.identity, hitChanceObject.transform.parent);
            if (sliderPos.GetComponent<RectTransform>().position.x >= hitChanceObject.GetComponent<RectTransform>().position.x - hitChanceObject.GetComponent<RectTransform>().sizeDelta.x && sliderPos.GetComponent<RectTransform>().position.x <= hitChanceObject.GetComponent<RectTransform>().position.x)
            {
                DamageEnemy(battleInitiative[initiativeIndex].attack);
            }
            attackField.SetActive(false);
            SetNewAttacker();
        }
        sliderValue += sliderSpeed * Time.fixedDeltaTime;
        attackSlider.value = sliderValue;
    }

    private void DamageEnemy(int attack)
    {
        enemyHealth -= attack;
        enemyHealthUI.value = enemyHealth;
    }

    public void BeginEnemyAttack()
    {
        enemyAttackField.SetActive(true);
        enemyAttacks[Random.Range(0,enemyAttacks.Length)].SetActive(true);
        StartCoroutine(AttackTimer(5f));
        _state = GameState.EnemyAttack;
    }
    private void BattleControl()
    {
        Vector2 movement = new Vector2(0f, 0f);
        if (Input.GetKey(KeyCode.LeftArrow) && player.transform.position.x >= (enemyAttackField.transform.position.x - enemyAttackField.transform.lossyScale.x * 0.5) + borderOffset)
            movement += new Vector2(-1, 0);

        if (Input.GetKey(KeyCode.RightArrow) && player.transform.position.x <= (enemyAttackField.transform.position.x + enemyAttackField.transform.lossyScale.x * 0.5) - borderOffset)
            movement += new Vector2(1, 0);

        if (Input.GetKey(KeyCode.UpArrow) && player.transform.position.y <= (enemyAttackField.transform.position.y + enemyAttackField.transform.lossyScale.y * 0.5) - borderOffset)
            movement += new Vector2(0, 1);

        if (Input.GetKey(KeyCode.DownArrow) && player.transform.position.y >= (enemyAttackField.transform.position.y - enemyAttackField.transform.lossyScale.y * 0.5) + borderOffset)
            movement += new Vector2(0, -1);

        movement.Normalize();
        player.transform.Translate(movement * Time.deltaTime * moveSpeed);
    }

    private IEnumerator AttackTimer(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject attack in enemyAttacks)
            attack.SetActive(false);
        foreach (Transform bullet in ObjectPool.instance.transform)
            bullet.gameObject.SetActive(false);
        enemyAttackField.SetActive(false);
        SetActiveAttacker();
    }

    public void BeginSkillMenu()
    {
        skillField.SetActive(true);
        foreach (Transform oldSkill in skillArea.transform)
            Destroy(oldSkill.gameObject);
        foreach (string skill in battleInitiative[initiativeIndex].skills)
        {
            GameObject newSkill = Instantiate(skillPrefab);
            newSkill.transform.SetParent(skillArea.transform);
            newSkill.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = skill;
        }
        _state = GameState.SubMenu;
    }
    private void SubMenuControl()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseMenu();
        }
    }

    private void CloseMenu()
    {
        skillField.SetActive(false);
        _state = GameState.AttackMenu;
    }
}

public enum GameState
{
    AttackMenu,
    SubMenu,
    PlayerAttack,
    EnemyAttack
}
