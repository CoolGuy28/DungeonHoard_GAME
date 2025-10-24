using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedInteraction : Interactable
{
    private bool locked = true;
    [SerializeField] private Item requiredItem;

    public override void BeginDialogue(PartyObject player)
    {
        if (GameManager.instance.GetItemAmount(requiredItem) > 0)
        {
            GameManager.instance.UseItem(requiredItem);
            GameManager.instance.BeginDialogue(dialogue[1]);
            locked = false;
            gameObject.tag = "Staircase";
        }
        else if (locked)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
        else
        {
            
        }
    }
}
