using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Skill")]
public class Spell : Ability
{
    public int staminaCost;
    public MagicAction spellAttack;

    public override Action GetAction()
    {
        return spellAttack;
    }
}
