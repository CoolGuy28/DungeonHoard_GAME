using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/BaseUnit")]
public class Unit : ScriptableObject
{
    public Stats baseStats;
    public Item_Weapon weapon;
    public List<Skill> skills;
    //public Condition[] conditionImmunities;
    public Sprite battleSprite;
    public Sprite basicAttack;
    public Sprite[] attackSprites;
    public Sprite damageSprite;
    public Sprite downedSprite;
    public Sprite[] overWorldSprites;
    public Vector2 spriteSize = Vector2.one;

    /*public bool IsImmune(Condition con)
    {
        bool immune = false;
        foreach (Condition c in conditionImmunities)
            if (con = c)
                immune = true;

        return immune;
    }*/
}
