using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "Condition/BaseCondition")]
public class Condition : ScriptableObject
{
    public Sprite[] sprite;
    [TextArea] public string desc;
    public virtual void OnConditionGained(BattleCharObject battleCharObject, int level)
    {
    }

    public virtual void OnConditionTick(BattleCharObject battleCharObject, int level)
    {
    }

    public virtual void OnConditionEnd(BattleCharObject battleCharObject, int level)
    {
        battleCharObject.GetCharacter().RemoveCondition(this);
    }

    public Sprite GetSprite(int level)
    {
        if (level < sprite.Length && level >= 0)
            return sprite[level];
        else
            return null;
    }
}