using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/BossUnit")]
public class Unit_Boss : Unit
{
    [Header("Phase 2")]
    public Stats baseStats_Phase2;
    public Item_Weapon weapon_Phase2;
    public List<Skill> skills_Phase2;
    public Sprite battleSprite_Phase2;
    public Sprite basicAttack_Phase2;
    public Sprite[] attackSprites_Phase2;
    public Sprite damageSprite_Phase2;
    public Sprite downedSprite_Phase2;
    public AudioClip transformSound;
}
