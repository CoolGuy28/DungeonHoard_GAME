using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialManager : MonoBehaviour
{
    private bool hasMoved = false;
    [SerializeField] private TMP_Text tutText;
    [SerializeField] private Animator animator;
    [SerializeField] private string movementText;
    private void Start()
    {
        StartCoroutine(DisplayMovement(3));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            hasMoved = true;
        }
    }

    private IEnumerator DisplayMovement(float timer)
    {
        yield return new WaitForSeconds(timer);
        if (!hasMoved)
        {
            tutText.text = movementText;
            animator.Rebind();
            animator.Play("TutorialTextFade");
            StartCoroutine(DisplayMovement(10));
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
