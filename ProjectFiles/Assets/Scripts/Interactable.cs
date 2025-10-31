using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IDataPersistence
{
    private Vector2 startingPos;
    public DialogueSection[] dialogue;
    private float interactCooldown = 0.4f;
    private bool canInteract = true;
    public bool used = false;

    private void Awake()
    {
        startingPos = transform.position;
    }
    private void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<SpriteRenderer>())
                t.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(transform.position.y - 0.25f) * -1;
        }
    }

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

    public void LoadData(GameData data)
    {
        InteractableData foundCopy = null;
        foreach (InteractableData i in data.sceneData[data.sceneIndex].interactions)
        {
            if (i.SameStartingPos(startingPos) != null)
            {
                foundCopy = i;
                break;
            }
        }
        if (foundCopy == null)
        {
            data.sceneData[data.sceneIndex].interactions.Add(new InteractableData(startingPos, used));
        }
        else
        {
            used = foundCopy.used;
        }
    }

    public void SaveData(GameData data)
    {
        InteractableData foundCopy = null;
        foreach (InteractableData i in data.sceneData[data.sceneIndex].interactions)
        {
            if (i.SameStartingPos(startingPos) != null)
            {
                foundCopy = i;
                break;
            }
        }
        if (foundCopy == null)
        {
            data.sceneData[data.sceneIndex].interactions.Add(new InteractableData(startingPos, used));
        }
        else
        {
            foundCopy = new InteractableData(startingPos, used);
        }
    }
}
