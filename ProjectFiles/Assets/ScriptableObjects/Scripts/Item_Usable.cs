using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UsableItem", menuName = "Item/UsableItem")]
public class Item_Usable : Item
{
    public Action action;

    public override Action GetAction()
    {
        return action;
    }
}