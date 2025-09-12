using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public DialogueSection[] dialogue;
    private float interactCooldown = 0.15f;
    private bool canInteract = true;

    public void TryDialogue(PartyObject player)
    {
        if (canInteract)
        {
            BeginDialogue(player);
            StartCoroutine(InteractCooldown());
        }

    }
    public virtual void BeginDialogue(PartyObject player)
    {
        if (canInteract)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
    }

    public IEnumerator InteractCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }
}
