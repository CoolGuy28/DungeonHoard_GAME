using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public Unit unit;
    public bool downed;
    public bool phase2;
    public Stats currentStats;
    public int currentHealth;
    public Item_Weapon weapon;
    public List<Skill> skills;
    public List<ConditionStats> conditions = new List<ConditionStats>();

    public CharacterData(Unit unit, Item_Weapon weapon)
    {
        this.unit = unit;
        this.weapon = weapon;
        InitialiseChar();
    }
    public void InitialiseChar()
    {
        downed = false;
        conditions.Clear();
        currentStats = new Stats(unit.baseStats);
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
        else
        {
            downed = false;
        }
        if (currentHealth > currentStats.maxHealth)
        {
            currentHealth = currentStats.maxHealth;
        }
    }

    public void AdjustStats(Stats statAdjustment, int multiplyer)
    {
        currentStats.attack += statAdjustment.attack * multiplyer;
        currentStats.maxHealth += statAdjustment.maxHealth * multiplyer;
        currentStats.defence += statAdjustment.defence * multiplyer;
        currentStats.healingEffect += statAdjustment.healingEffect * multiplyer;
        currentStats.fireRes += statAdjustment.fireRes * multiplyer;
        currentStats.poisonRes += statAdjustment.poisonRes * multiplyer;
        currentStats.coldRes += statAdjustment.coldRes * multiplyer;
        currentStats.accuracy += statAdjustment.accuracy * multiplyer;
        currentStats.speed += statAdjustment.speed * multiplyer;
        currentStats.critPercent += statAdjustment.critPercent * multiplyer;
        currentStats.critMultiplyer += statAdjustment.critMultiplyer * multiplyer;
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
public class Stats
{
    public float attack = 1;
    public int maxHealth = 100;
    public float defence = 1;
    public float healingEffect = 1;
    public float fireRes = 1;
    public float poisonRes = 1;
    public float coldRes = 1;
    public float accuracy = 1;
    public float speed = 1;
    public float critPercent = 0.1f;
    public float critMultiplyer = 1.6f;

    public Stats(Stats stats)
    {
        this.attack = stats.attack;
        this.maxHealth = stats.maxHealth;
        this.defence = stats.defence;
        this.healingEffect = stats.healingEffect;
        this.fireRes = stats.fireRes;
        this.poisonRes = stats.poisonRes;
        this.coldRes = stats.coldRes;
        this.accuracy = stats.accuracy;
        this.speed = stats.speed;
        this.critPercent = stats.critPercent;
        this.critMultiplyer = stats.critMultiplyer;
    }
}

[System.Serializable]
public class Action
{
    public TargetingType targetingType;
    public DamageStats damageStats;
    public float accuracy;
    public bool useCrit;
    public int staminaAdjust;
    public int spriteIndex;
    public AudioClip hitSFX;
    public AudioClip missSFX;
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

    public ItemSlot(Item item, int quantity)
    {
        this.item =  item;
        this.quantity = quantity;
    }
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

[System.Serializable]
public class DamageStats
{
    public int damage;
    public DamageType damageType;
    public int randomVarience;
    public GameObject particleEffect;
    public ConditionStats[] applyConditions;
    public int conditionChance;
    public Condition[] removeConditions;

    public DamageStats(DamageStats stats)
    {
        damage = stats.damage;
        damageType = stats.damageType;
        randomVarience = stats.randomVarience;
        particleEffect = stats.particleEffect;
        applyConditions = stats.applyConditions;
        removeConditions = stats.removeConditions;
        conditionChance = stats.conditionChance;
    }
}

public enum DamageType
{
    Physical,
    Magic,
    Healing,
    Fire,
    Poison,
    Cold
}

[System.Serializable]
public class OverworldScene
{
    public List<EnemyOverworldData> enemies = new List<EnemyOverworldData>();
    public void SaveNewEnemy(EnemyOverworldData enemy)
    {
        enemies.Add(enemy);
    }
}

[System.Serializable]
public class EnemyOverworldData
{
    public List<CharacterData> enemyFight;
    public Vector2 enemyPosition;
    public bool dead;
    public int displaySprite;
    public void SetOverworldData(EnemyOverworldData data)
    {
        enemyFight = data.enemyFight;
        enemyPosition = data.enemyPosition;
        dead = data.dead;
        displaySprite = data.displaySprite;
    }
}