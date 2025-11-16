using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public Unit unit;
    public bool downed;
    public Stats baseStats;
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
        baseStats = new Stats(unit.baseStats);
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

    public void SetCurrentStats()
    {
        currentStats = new Stats(baseStats);
        foreach(ConditionStats c in conditions)
        {
            Condition_StatChange con = c.condition as Condition_StatChange;
            AdjustStats(con.statAdjust, 1);
        }
    }

    public void AdjustStats(Stats statAdjustment, int multiplyer)
    {
        currentStats.attack += statAdjustment.attack * multiplyer;
        if (currentStats.attack <= 0.2)
            currentStats.attack = 0.2f;
        currentStats.maxHealth += statAdjustment.maxHealth * multiplyer;
        currentStats.defence += statAdjustment.defence * multiplyer;
        if (currentStats.defence <= 0.2)
            currentStats.defence = 0.2f;
        currentStats.healingEffect += statAdjustment.healingEffect * multiplyer;
        if (currentStats.healingEffect <= 0.2)
            currentStats.healingEffect = 0.2f;
        currentStats.fireRes += statAdjustment.fireRes * multiplyer;
        if (currentStats.fireRes <= 0)
            currentStats.fireRes = 0;
        currentStats.poisonRes += statAdjustment.poisonRes * multiplyer;
        if (currentStats.poisonRes <= 0)
            currentStats.poisonRes = 0;
        currentStats.coldRes += statAdjustment.coldRes * multiplyer;
        if (currentStats.coldRes <= 0)
            currentStats.fireRes = 0;
        currentStats.accuracy += statAdjustment.accuracy * multiplyer;
        currentStats.speed += statAdjustment.speed * multiplyer;
        currentStats.critPercent += statAdjustment.critPercent * multiplyer;
        currentStats.critMultiplyer += statAdjustment.critMultiplyer * multiplyer;
        currentStats.increaseTargetChance += statAdjustment.increaseTargetChance * multiplyer;
        currentStats.actions += statAdjustment.actions * multiplyer;
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
        if (obj != null)
        {
            addCondition.condition.OnConditionGained(obj, addCondition.level);
        }
        SetCurrentStats();
    }

    public bool FindCondition(Condition searchCondition)
    {
        foreach (ConditionStats c in conditions)
        {
            if (c.condition == searchCondition)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveCondition(Condition removeCondition)
    {
        foreach (ConditionStats c in conditions)
        {
            if (c.condition == removeCondition)
            {
                conditions.Remove(c);
                SetCurrentStats();
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
    public int increaseTargetChance = 0;
    public int actions = 1;
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
        this.increaseTargetChance = stats.increaseTargetChance;
        this.actions = stats.actions;
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
    public bool applySelfInjury;
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
    public bool applySingleRandCondition;
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
    public List<InteractableData> interactions = new List<InteractableData>();
    public void SaveNewEnemy(EnemyOverworldData enemy)
    {
        enemies.Add(new EnemyOverworldData(enemy));
    }
}

[System.Serializable]
public class EnemyOverworldData
{
    public List<CharacterData> enemyFight;
    public Vector2 startingPos;
    public Vector2 enemyPosition;
    public bool dead;
    public int displaySprite;
    public Sprite combatBackground;
    public EnemyOverworldData(EnemyOverworldData data)
    {
        startingPos = data.startingPos;
        enemyFight = data.enemyFight;
        enemyPosition = data.enemyPosition;
        dead = data.dead;
        displaySprite = data.displaySprite;
        combatBackground = data.combatBackground;
    }

    public void Copy(EnemyOverworldData data)
    {
        startingPos = data.startingPos;
        enemyFight = data.enemyFight;
        enemyPosition = data.enemyPosition;
        dead = data.dead;
        displaySprite = data.displaySprite;
        combatBackground = data.combatBackground;
    }

    public EnemyOverworldData SameStartingPos(Vector2 vector2)
    {
        if (vector2 == startingPos)
            return this;
        else
            return null;
    }
}

[System.Serializable]
public class InteractableData
{
    public Vector2 startingPos;
    public bool used;
    public InteractableData(Vector2 startPos, bool used)
    {
        this.startingPos = startPos;
        this.used = used;
    }

    public void SetUsed(bool use)
    {
        used = use;
    }

    public InteractableData SameStartingPos(Vector2 vector2)
    {
        if (vector2 == startingPos)
            return this;
        else
            return null;
    }
}