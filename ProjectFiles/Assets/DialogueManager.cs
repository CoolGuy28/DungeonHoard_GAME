using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject DialoguePanel;
    [SerializeField] private TMP_Text DialogueText;
    [SerializeField] private TMP_Text DialogueSpeaker;
    public bool inDialogue;
    [SerializeField] private DialogueSection currentDialogue;
    private int currentLine;

    private void Start()
    {
        DialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (inDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (currentLine < currentDialogue.dialogue.Length)
                {
                    LoadDialogueLine(currentDialogue.dialogue[currentLine]);
                }
                else
                {
                    GameManager.instance.EndDialogue();
                    inDialogue = false;
                    GameObject.FindObjectOfType<PartyObject>().EndFishing();
                    DialoguePanel.SetActive(false);
                }
            }
        }
    }
    public void LoadDialogue(DialogueSection section)
    {
        currentLine = 0;
        currentDialogue = section;
        inDialogue = true;
        GameObject.FindObjectOfType<PartyObject>().BeginFishing();
        DialoguePanel.SetActive(true);
        LoadDialogueLine(section.dialogue[0]);
    }

    private void LoadDialogueLine(DialogueLine line)
    {
        DialogueSpeaker.text = line.name;
        DialogueText.text = line.text;
        currentLine++;
    }
}

[System.Serializable]
public class DialogueSection
{
    public DialogueLine[] dialogue;
}

[System.Serializable]
public class DialogueLine
{
    public string name;
    public string text;
}
