using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedInteraction : Interactable
{
    [SerializeField] private Item requiredItem;
    [SerializeField] private int sceneChange;
    [SerializeField] private Vector2 position;

    public override void BeginDialogue(PartyObject player)
    {
        if (!used && GameManager.instance.GetItemAmount(requiredItem) > 0)
        {
            GameManager.instance.UseItem(requiredItem);
            GameManager.instance.BeginDialogue(dialogue[1]);
            used = true;
            gameObject.tag = "Staircase";
        }
        else if (!used)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
        else if (used)
        {
            GameManager.instance.ChangeGameScene(sceneChange);
            GameManager.instance.gameData.playerPos = position;
        }
    }
}
