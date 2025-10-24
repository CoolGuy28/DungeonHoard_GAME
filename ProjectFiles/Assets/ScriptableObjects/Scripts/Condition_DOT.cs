using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDOT", menuName = "Condition/DamageOverTime")]
public class Condition_DOT : Condition_StatChange
{
    public DamageStats[] damagePerTurn;

    public override void OnConditionTick(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.TakeDamage(damagePerTurn[level],false, null);
    }

    public override void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.TakeDamage(damagePerTurn[level],false, null);
        battleCharObject.GetCharacter().AdjustStats(statAdjust, -1);
        battleCharObject.GetCharacter().RemoveCondition(this);
    }
}
