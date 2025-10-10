using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : Interactable
{
    [SerializeField] private List<ItemSlot> itemPool;
    private bool looted;
    public ButtonEvent onLootEvent;
    public override void BeginDialogue(PartyObject player)
    {
        if (looted)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
        else
        {
            string lootText = "";
            ItemSlot slot = itemPool[Random.Range(0, itemPool.Count)];
            if (slot.quantity > 0 && slot.item != null)
            {
                lootText = "You find " + slot.quantity.ToString() + " " + slot.item.name;
                GameManager.instance.AddItem(slot.item, slot.quantity);
                onLootEvent.Invoke();
            }
            else
                lootText = "You find nothing";

            DialogueSection lootDia = new DialogueSection(" ", lootText);
            GameManager.instance.BeginDialogue(lootDia);
            looted = true;
        }
    }
}
