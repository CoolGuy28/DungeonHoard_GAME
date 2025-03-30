using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowItem", menuName = "Item/ThrowableItem")]
public class ThrowableItem : Item
{
    public PhysicalAction attack;

    public override Action GetAction()
    {
        return attack;
    }
}
