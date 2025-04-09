using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public List<BaseCharacter> party;
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

        foreach (BaseCharacter character in party)
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
    public BaseUnit unit;
    public bool downed;
    public Stats currentStats;
    public int currentHealth;
    public Weapon weapon;
    public List<Ability> skills;
    public List<ConditionStats> conditions = new List<ConditionStats>();
    
    public void InitialiseChar()
    {
        currentStats = unit.baseStats;
        weapon = unit.weapon;
        skills = unit.skills;
        currentHealth = currentStats.maxHealth;
    }
    public void AdjustHealth(int value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            downed = true;
            currentHealth = 0;
        }
        if (currentHealth > currentStats.maxHealth)
        {
            currentHealth = currentStats.maxHealth;
        }
    }

    public void AddCondition(ConditionStats addCondition)
    {
        foreach (ConditionStats c in conditions)
        {
            if (c.condition == addCondition.condition)
            {
                if (addCondition.timeFrame == -1 || c.timeFrame < addCondition.timeFrame)
                    c.timeFrame = addCondition.timeFrame;
                if (addCondition.level > c.level)
                    c.level = addCondition.level;
                return;
            }
        }
        conditions.Add(addCondition);
    }

    public void RemoveCondition(Condition removeCondition)
    {
        foreach (ConditionStats c in conditions)
        {
            if (c.condition == removeCondition)
            {
                conditions.Remove(c);
                return;
            }
        }
    }
}

[System.Serializable]
public class Action
{
    public TargetingType targetingType;
    public int damage;
    public bool healAction;
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
public class ConditionStats
{
    public Condition condition;
    public int timeFrame;
    public int level;

    public ConditionStats(Condition condition, int timeFrame, int level)
    {
        this.condition = condition;
        this.timeFrame = timeFrame;
        this.level = level;
    }
}
