using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDOT", menuName = "Condition/DamageOverTime")]
public class Condition_DOT : Condition
{
    public int[] damagePerTurn;
    public Color damageColor = Color.white;

    public override void OnConditionTick(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.TakeDamage(damagePerTurn[level], damageColor);
    }

    public override void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.TakeDamage(damagePerTurn[level], damageColor);
        battleCharObject.GetCharacter().RemoveCondition(this);
    }
}
