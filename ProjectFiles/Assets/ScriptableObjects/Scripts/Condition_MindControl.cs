using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_MindControl", menuName = "Condition/MindControl")]
public class Condition_MindControl : Condition
{
    /*public override void OnConditionGained(BattleCharObject battleCharObject, int level)
    {
        GameObject.Find("Canvas").GetComponent<BattleManager>().SwitchCharacterTeam(battleCharObject.GetCharacter());
    }
    public override void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        GameObject.Find("Canvas").GetComponent<BattleManager>().SwitchCharacterTeam(battleCharObject.GetCharacter());
        battleCharObject.GetCharacter().RemoveCondition(this);
    }*/
}
