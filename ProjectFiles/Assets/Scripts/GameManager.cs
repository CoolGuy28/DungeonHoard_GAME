using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<CharacterData> party;
    public List<ItemSlot> items;
    public int maxStamina;
    public int stamina;
    public Vector2 partyPosition;
    [SerializeField] private Animator animator;
    private int currentEnemyIndex;
    [SerializeField] private EnemyOverworldData[] overworldEnemies;
    [SerializeField] private OverworldEnemyObject enemyObjectPrefab;

    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
                Destroy(gameObject);
        }

        foreach (CharacterData character in party)
            character.InitialiseChar();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SpawnEnemyObjects();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SpawnEnemyObjects();
        }
    }

    public void AddItem(Item item)
    {
        bool itemAdded = false;
        foreach (ItemSlot i in items)
        {
            if (i.item == item)
            {
                itemAdded = true;
                i.quantity++;
                break;
            }  
        }
        if (!itemAdded)
        {
            items.Add(new ItemSlot(item, 1));
        }
    }

    public void UseItem(Item item)
    {
        foreach (ItemSlot i in items)
        {
            if (i.item == item)
            {
                i.quantity--;
                if (i.quantity <= 0)
                    items.Remove(i);
                break;
            }
        }
    }

    public int GetItemAmount(Item item)
    {
        foreach (ItemSlot i in items)
        {
            if (i.item == item)
            {
                return i.quantity;
            }
        }
        return 0;
    }

    public List<Ability> GetItemList()
    {
        List<Ability> list = new List<Ability>();
        foreach (ItemSlot i in items)
        {
            list.Add(i.item);
        }
        return list;
    }

    public void AdjustStamina(int adjustAmount)
    {
        stamina += adjustAmount;
        if (stamina < 0)
            stamina = 0;
        if (stamina > maxStamina)
            stamina = maxStamina;
    }

    private void SpawnEnemyObjects()
    {
        for (int i = 0; i < overworldEnemies.Length; i++)
        {
            Instantiate(enemyObjectPrefab, overworldEnemies[i].enemyPosition, Quaternion.identity).SetEnemyData(overworldEnemies[i], i);
        }
    }

    public List<CharacterData> GetActiveEnemyList()
    {
        return overworldEnemies[currentEnemyIndex].enemyFight;
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadScene(0));
    }

    public void LoadGame(GameManager gameManager)
    {
        this.party = gameManager.party;
        this.items = gameManager.items;
        this.maxStamina = gameManager.maxStamina;
        this.stamina = gameManager.stamina;
        this.partyPosition = gameManager.partyPosition;
        foreach (CharacterData unit in party)
            unit.InitialiseChar();
        StartCoroutine(LoadScene(1));
    }

    public void LoadBattle(int currentEnemyIndex)
    {
        this.currentEnemyIndex = currentEnemyIndex;
        StartCoroutine(LoadScene(2));
    }

    public void WonBattle()
    {
        overworldEnemies[currentEnemyIndex].dead = true;
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int scene)
    {
        animator.SetBool("Fade", true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(scene);
        animator.SetBool("Fade", false);
    }
}

