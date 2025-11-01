using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private DialogueSection[] cutsceneText;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private GameObject[] tutorialObj;

    private void Awake()
    {
        if (GameManager.instance.gameData.seenTutorial)
        {
            foreach (GameObject t in tutorialObj)
                t.SetActive(false);
        }
    }

    public void CutsceneDialogue(int index)
    {
        GameManager.instance.BeginDialogue(cutsceneText[index]);
    }

    public void BeginTutorial()
    {
        tutorialManager.StartMoveDisplay();
        GameManager.instance.gameData.seenTutorial = true;
    }
}
