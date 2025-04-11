using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/BaseUnit")]
public class Unit : ScriptableObject
{
    public Stats baseStats;
    public Item_Weapon weapon;
    public List<Skill> skills;
    public Sprite battleSprite;
    public Sprite attackSprite;
    public Sprite damageSprite;
}
