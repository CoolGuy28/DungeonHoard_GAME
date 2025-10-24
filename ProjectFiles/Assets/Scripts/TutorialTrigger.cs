using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text tutText;
    [SerializeField] private Animator animator;
    [SerializeField] private string text;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutText.text = text;
            animator.Rebind();
            animator.Play("TutorialTextFade");
            this.gameObject.SetActive(false);
        }
    }
}
