using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Skill")]
public class Skill : Ability
{
    public int staminaCost;
    public Action action;

    public override Action GetAction()
    {
        return action;
    }
}
