using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class Weapon : Ability
{
    public PhysicalAction attack;

    public override Action GetAction()
    {
        return attack;
    }
}

