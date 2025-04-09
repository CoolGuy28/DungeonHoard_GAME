using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/BaseUnit")]
public class BaseUnit : ScriptableObject
{
    public Stats baseStats;
    public Weapon weapon;
    public List<Ability> skills;
    public Sprite battleSprite;
}
