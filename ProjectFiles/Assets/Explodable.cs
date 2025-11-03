using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Interactable
{
    [SerializeField] private Item requiredItem;
    [SerializeField] private Vector2 position;

    public override void BeginDialogue(PartyObject player)
    {
        if (!used && GameManager.instance.GetItemAmount(requiredItem) > 0)
        {
            GameManager.instance.UseItem(requiredItem);
            GameManager.instance.BeginDialogue(dialogue[1]);
            used = true;
            gameObject.tag = "Staircase";
            GameManager.instance.SaveGame();
            StartCoroutine(ExplodeTimer());
        }
        else if (!used)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
    }

    private IEnumerator ExplodeTimer()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    public override void OnLoadInteractable()
    {
        if (used)
            gameObject.SetActive(false);
    }
}
