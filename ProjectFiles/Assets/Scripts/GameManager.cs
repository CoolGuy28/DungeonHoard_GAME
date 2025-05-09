using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<CharacterData> party;
    public List<CharacterData> enemies;
    public List<ItemSlot> items;
    public int maxStamina;
    public int stamina;
    public Vector2 partyPosition;
    [SerializeField] private Animator animator;

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

    public void LoadBattle(OverworldEnemyObject enemies)
    {
        this.enemies = enemies.GetEnemies();
        StartCoroutine(LoadScene(2));
    }

    public void WonBattle()
    {
        StartCoroutine(LoadScene(1));
    }

    private IEnumerator LoadScene(int scene)
    {
        animator.SetBool("Fade", true);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadSceneAsync(scene);
        animator.SetBool("Fade", false);
    }
}

