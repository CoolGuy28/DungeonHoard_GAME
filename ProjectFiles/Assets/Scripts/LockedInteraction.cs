using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedInteraction : Interactable
{
    [SerializeField] private Item requiredItem;
    [SerializeField] private int itemQuant = 1;
    [SerializeField] private bool changeScene;
    [SerializeField] private int sceneChange;
    [SerializeField] private Vector2 position;
    [SerializeField] private LockedInteraction linked;
    private AudioSource audioSource;
    [SerializeField] AudioClip openSound;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void BeginDialogue(PartyObject player)
    {
        if (!used && GameManager.instance.GetItemAmount(requiredItem) >= itemQuant)
        {
            GameManager.instance.UseItem(requiredItem);
            GameManager.instance.BeginDialogue(dialogue[1]);
            used = true;
            if (linked != null)
                linked.used = true;
            gameObject.tag = "Staircase";
            if (audioSource != null && openSound != null)
            {
                audioSource.clip = openSound;
                audioSource.Play();
            }
        }
        else if (!used)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
        else if (used)
        {
            if (changeScene)
                GameManager.instance.ChangeGameScene(sceneChange);
            else
                GameManager.instance.DoTransition();
            GameManager.instance.gameData.playerPos = position;
        }
    }
}
