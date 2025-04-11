using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public Unit unit;
    public bool downed;
    public Stats currentStats;
    public int currentHealth;
    public Item_Weapon weapon;
    public List<Skill> skills;
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
        currentHealth -= (int)(value);
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

    public void AdjustStats(Stats statAdjustment, int multiplyer)
    {
        currentStats.maxHealth += statAdjustment.maxHealth * multiplyer;
        currentStats.defence += statAdjustment.defence * multiplyer;
        currentStats.healingEffect += statAdjustment.healingEffect * multiplyer;
        currentStats.attack += statAdjustment.attack * multiplyer;
        currentStats.accuracy += statAdjustment.accuracy * multiplyer;
        currentStats.critChance += statAdjustment.critChance * multiplyer;
    }

    public void AddCondition(ConditionStats addCondition, BattleCharObject obj)
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
        addCondition.condition.OnConditionGained(obj, addCondition.level);
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

    public List<Ability> GetSkills()
    {
        List<Ability> skillAbilities = new List<Ability>();
        foreach (Skill skill in skills)
            skillAbilities.Add(skill);
        return skillAbilities;
    }
}

[System.Serializable]
public struct Stats
{
    public int attack;
    public int maxHealth;
    public float defence;
    public float healingEffect;
    public float accuracy;
    public float critChance;
}

[System.Serializable]
public class Action
{
    public TargetingType targetingType;
    public bool healAction;
    public int damage;
    public GameObject particleEffect;
    public ConditionStats[] applyConditions;
    public int conditionChance;
    public Condition[] removeConditions;
    public float accuracy;
    public float critChance;
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
