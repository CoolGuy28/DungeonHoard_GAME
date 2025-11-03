using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : Interactable
{
    [SerializeField] private List<ItemSlot> itemPool;
    [SerializeField] private Sprite lootedSprite;
    public ButtonEvent onLootEvent;
    public override void BeginDialogue(PartyObject player)
    {
        if (used)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
        else
        {
            string lootText = "";
            ItemSlot slot = itemPool[Random.Range(0, itemPool.Count)];
            if (slot.quantity > 0 && slot.item != null)
            {
                lootText = "You find <color=#bd2d28>" + slot.quantity.ToString() + " " + slot.item.name;
                if (slot.quantity > 1)
                    lootText += "s";
                lootText += "</color>";
                GameManager.instance.AddItem(slot.item, slot.quantity);
                onLootEvent.Invoke();
            }
            else
                lootText = "You find nothing";

            DialogueSection lootDia = new DialogueSection(" ", lootText);
            GameManager.instance.BeginDialogue(lootDia);
            used = true;
            if (lootedSprite != null)
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = lootedSprite;
        }
    }

    public override void OnLoadInteractable()
    {
        if (used)
        {
            if (lootedSprite != null)
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = lootedSprite;
        }
    }
}
