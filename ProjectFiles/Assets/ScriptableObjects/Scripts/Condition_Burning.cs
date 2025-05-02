using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Burning", menuName = "Condition/Fire")]
public class Condition_Burning : Condition_DOT
{
    public override void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.TakeDamage(damagePerTurn[level]);
        if (level < 2)
        {
            battleCharObject.GetCharacter().AddCondition(new ConditionStats(this, 3, level + 1), battleCharObject);
        }
        else
        {
            battleCharObject.GetCharacter().RemoveCondition(this);
        }
    }
}
