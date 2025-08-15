using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private DialogueSection dialogue;
    public void BeginDialogue()
    {
        GameManager.instance.BeginDialogue(dialogue);
    }
}
