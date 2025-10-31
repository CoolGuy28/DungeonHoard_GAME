using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject DialoguePanel;
    [SerializeField] private TMP_Text DialogueText;
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
                if (currentLine < currentDialogue.dialogue.Count)
                {
                    LoadDialogueLine(currentDialogue.dialogue[currentLine]);
                }
                else
                {
                    GameManager.instance.EndDialogue();
                    inDialogue = false;
                    GameObject.FindObjectOfType<PartyObject>().AllowMovement();
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
        GameObject.FindObjectOfType<PartyObject>().PauseMovement();
        DialoguePanel.SetActive(true);
        LoadDialogueLine(section.dialogue[0]);
    }
    private void LoadDialogueLine(DialogueLine line)
    {
        string dialogueLine = "";
        if (line.name != null && line.name.Trim() != "")
        {
            DialogueText.alignment = TextAlignmentOptions.Top;
            dialogueLine += "<size=42>" + line.name + "</size>\n";
        }
        else
            DialogueText.alignment = TextAlignmentOptions.Center;
        dialogueLine += line.text;
        DialogueText.text = dialogueLine;
        currentLine++;
    }
}

[System.Serializable]
public class DialogueSection
{
    public List<DialogueLine> dialogue;

    public DialogueSection(string name, string text)
    {
        dialogue = new List<DialogueLine>();
        DialogueLine line = new DialogueLine(name, text);
        dialogue.Add(line);
    }
}

[System.Serializable]
public class DialogueLine
{
    public string name;
    public string text;

    public DialogueLine(string name, string text)
    {
        this.name = name;
        this.text = text;
    }
}
