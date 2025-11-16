using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSection dialogue;
    [SerializeField] private TMP_FontAsset font;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.BeginDialogue(dialogue, font);
            gameObject.SetActive(false);
        }
    }
}
