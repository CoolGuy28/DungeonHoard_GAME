using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Skill/MagicSkill")]
public class Skill_Magic : Skill
{
    public int corruption;
    public PatternSettings magicPattern;
    public float castingSpeed;
}
