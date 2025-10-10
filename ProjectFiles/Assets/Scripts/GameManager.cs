using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    [SerializeField] private bool initializeDataIfNull = false;
    public GameData gameData;
    public List<CharacterData> party = new List<CharacterData>();
    public List<ItemSlot> items = new List<ItemSlot>();
    public int maxStamina;
    public int stamina;
    public Vector2 partyPosition;
    [SerializeField] private Animator animator;
    private int currentEnemyIndex;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject menu;
    private List<IDataPersistence> dataPersistenceObjects;
    [SerializeField] private List<CharacterData> newGameParty;
    [SerializeField] private List<ItemSlot> newGameItems;
    public bool saved;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        foreach (CharacterData character in party)
            character.InitialiseChar();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 2)
        {
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
        }
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 2)
        {
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void AddItem(Item item, int quant)
    {
        bool itemAdded = false;
        foreach (ItemSlot i in items)
        {
            if (i.item == item)
            {
                itemAdded = true;
                i.quantity += quant;
                break;
            }
        }
        if (!itemAdded)
        {
            items.Add(new ItemSlot(item, quant));
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

    public void BeginDialogue(DialogueSection section)
    {
        Time.timeScale = 0;
        dialogueManager.LoadDialogue(section);
    }
    public void EndDialogue()
    {
        Time.timeScale = 1;
    }

    public void NewGame()
    {
        gameData = new GameData(newGameParty, newGameItems, Vector2.zero);
        foreach (CharacterData unit in party)
            unit.InitialiseChar();
    }
    public void NewGame(List<CharacterData> party, List<ItemSlot> items)
    {
        Debug.Log("NewGame");
        gameData = new GameData(party, items, GameObject.Find("PartyObject").transform.position);
        foreach (CharacterData unit in party)
            unit.InitialiseChar();
    }

    public void LoadMenu()
    {
        CloseOverworldMenu();
        StartCoroutine(ChangeScene(0));
    }

    public void LoadGame()
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (gameData.party.Count == 0 && initializeDataIfNull)
        {
            NewGame();
        }
        Debug.Log("SceneLoading");
        gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        maxStamina = gameData.maxStamina;
        stamina = maxStamina;
        

        party = gameData.party;
        items = gameData.items;

        if (gameData.sceneData[gameData.sceneIndex].enemies.Count() == 0 && GameObject.Find("PartyObject"))
        {
            gameData.playerPos = GameObject.Find("PartyObject").transform.position;
        }

        partyPosition = gameData.playerPos;

        for (int i = 0; i < dataPersistenceObjects.Count; i++)
        {
            dataPersistenceObjects[i].LoadData(gameData, i);
        }

        GameObject.Find("PartyObject").transform.position = partyPosition;
        PauseGame(false);
    }

    public void SaveGame()
    {
        Debug.Log("SceneSaved");
        saved = true;
        gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        gameData.maxStamina = maxStamina;
        partyPosition = GameObject.Find("PartyObject").transform.position;
        gameData.playerPos = partyPosition;
        gameData.party = party;
        gameData.items = items;

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
    }

    public void OpenOverworldMenu()
    {
        PauseGame(true);
        menu.SetActive(true);
    }

    public void CloseOverworldMenu()
    {
        PauseGame(false);
        menu.SetActive(false);
        GameObject.Find("PartyObject").GetComponent<PartyObject>().EndFishing();
    }

    private void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public void LoadBattle(int currentEnemyIndex)
    {
        if (!gameData.sceneData[gameData.sceneIndex].enemies[currentEnemyIndex].dead)
        {
            this.currentEnemyIndex = currentEnemyIndex;
            gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(ChangeScene("BattleScene"));
        }
    }

    public List<CharacterData> GetActiveEnemyList()
    {
        return gameData.sceneData[gameData.sceneIndex].enemies[currentEnemyIndex].enemyFight;
    }

    public void WonBattle()
    {
        gameData.sceneData[gameData.sceneIndex].enemies[currentEnemyIndex].dead = true;
        StartCoroutine(ChangeScene(gameData.sceneIndex));
    }

    public void ChangeGameScene(int sceneIndex)
    {
        StartCoroutine(ChangeScene(sceneIndex));
    }

    private IEnumerator ChangeScene(int scene)
    {
        animator.SetBool("Fade", true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(scene);
        animator.SetBool("Fade", false);
    }

    private IEnumerator ChangeScene(string name)
    {
        animator.SetBool("Fade", true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(name);
        animator.SetBool("Fade", false);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}

[System.Serializable]
public class GameData
{
    public List<CharacterData> party;
    public List<ItemSlot> items;
    public int maxStamina;
    public Vector2 playerPos;
    public int sceneIndex;
    public OverworldScene[] sceneData;

    public GameData()
    {
        maxStamina = 100;
        sceneData = new OverworldScene[5];
        for (int i = 0; i < sceneData.Length; i++)
        {
            sceneData[i] = new OverworldScene();
        }
    }

    public GameData(List<CharacterData> party)
    {
        maxStamina = 100;
        this.party = party;
        sceneData = new OverworldScene[5];
        for (int i = 0; i < sceneData.Length; i++)
        {
            sceneData[i] = new OverworldScene();
        }
    }

    public GameData(List<CharacterData> party, List<ItemSlot> items, Vector2 startPos)
    {
        maxStamina = 100;
        this.party = new List<CharacterData>();
        this.items = new List<ItemSlot>();
        foreach (CharacterData cha in party)
        {
            this.party.Add(new CharacterData(cha.unit,cha.weapon));
        }
        foreach (ItemSlot item in items)
        {
            this.items.Add(new ItemSlot(item.item, item.quantity));
        }
        playerPos = startPos;
        sceneData = new OverworldScene[5];
        for (int i = 0; i < sceneData.Length; i++)
        {
            sceneData[i] = new OverworldScene();
        }
    }
}