using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionStat", menuName = "Condition/StatChange")]
public class Condition_StatChange : Condition 
{
    public Stats statAdjust;
    public override void OnConditionGained(BattleCharObject battleCharObject, int level)
    {
        //battleCharObject.GetCharacter().AdjustStats(statAdjust, 1);
    }

    public override void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.GetCharacter().SetCurrentStats();
        battleCharObject.GetCharacter().RemoveCondition(this);
    }
}
