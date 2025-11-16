using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Interactable
{
    [SerializeField] private Item requiredItem;
    [SerializeField] private Vector2 position;
    private AudioSource audioSource;
    [SerializeField] AudioClip fuseSound;
    [SerializeField] private GameObject explodeSound;
    [SerializeField] private GameObject explosionVFX;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void BeginDialogue(PartyObject player)
    {
        if (!used && GameManager.instance.GetItemAmount(requiredItem) > 0)
        {
            GameManager.instance.UseItem(requiredItem);
            GameManager.instance.BeginDialogue(dialogue[1]);
            used = true;
            gameObject.tag = "Staircase";
            GameManager.instance.SaveGame();
            StartCoroutine(ExplodeTimer());
        }
        else if (!used)
        {
            GameManager.instance.BeginDialogue(dialogue[0]);
        }
    }

    private IEnumerator ExplodeTimer()
    {
        audioSource.clip = fuseSound;
        audioSource.Play();
        yield return new WaitForSeconds(3f);
        Instantiate(explosionVFX,transform.position, Quaternion.identity);
        Instantiate(explodeSound, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public override void OnLoadInteractable()
    {
        if (used)
            gameObject.SetActive(false);
    }
}
