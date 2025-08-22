using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private DialogueSection dialogue;
    private bool used;
    public void BeginDialogue()
    {
        if (!used)
        {
            GameManager.instance.BeginDialogue(dialogue);
            used = true;
        }
    }
}
