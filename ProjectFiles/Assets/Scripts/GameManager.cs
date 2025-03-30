using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<PlayerCharacter> party;
    public List<ItemSlot> items;
    public int maxStamina;
    public int stamina;

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

        foreach (PlayerCharacter character in party)
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
}

[System.Serializable]
public class BaseCharacter
{
    public string charName;
    public bool downed;
    public Stats baseStats;
    public Weapon weapon;
    public int currentHealth;
    public List<ConditionStats> conditions = new List<ConditionStats>();
    public Sprite battleSprite;
    public void InitialiseChar()
    {
        currentHealth = baseStats.maxHealth;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            downed = true;
        }
    }

    public void AddCondition(ConditionStats addCondition)
    {
        foreach (ConditionStats c in conditions)
        {
            if (c.condition == addCondition.condition)
                return;
        }
        conditions.Add(addCondition);
    }
}

[System.Serializable]
public class Action
{
    public TargetingType targetingType;
    public int damage;
    public GameObject particleEffect;
    public ConditionStats[] applyCondition;
    public int conditionChance;
}

[System.Serializable]
public class PhysicalAction : Action
{
    public float accuracy;
    public float critChance;
}

[System.Serializable]
public class MagicAction : Action
{
}

public class Ability : ScriptableObject
{
    [TextArea(5, 20)] public string description;
    
    public virtual Action GetAction()
    {
        return null;
    }
}

[System.Serializable]
public class PlayerCharacter : BaseCharacter
{
    public Sprite charPortrait;
    public List<Ability> skills;
}

[System.Serializable]
public struct Stats
{
    public int attack;
    public int maxHealth;
    public float accuracy;
    public float critChance;
}

[System.Serializable]
public class ItemSlot
{
    public Item item;
    public int quantity;
}


[System.Serializable]
public struct ConditionStats
{
    public Condition condition;
    public int timeFrame;
}
