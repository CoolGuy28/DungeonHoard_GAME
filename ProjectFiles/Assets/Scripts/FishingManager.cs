using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingManager : Interactable
{
    private bool currentlyFishing;
    [SerializeField] private GameObject fishingCanvas;
    private PartyObject fisher;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private Slider fishSlider;
    [SerializeField] private Slider captureSlider;
    [SerializeField] private Slider escapeSlider;
    [SerializeField] private float captureSliderSpeed;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float escapeSpeed;
    [SerializeField] private float fishMaxSpeed;
    [SerializeField] private float fishMinSpeed;
    private float currentFishSpeed;

    [SerializeField] private Item fishItem;
    [SerializeField] private GameObject textPrefab;
    private void BeginFishing()
    {
        fishingCanvas.SetActive(true);
        currentlyFishing=true;
        chargeSlider.value = 0;
        fishSlider.value = Random.Range(0f ,1f);
        captureSlider.value = 0.5f;
        escapeSlider.value = 0;
        StartCoroutine(FishMovement(Random.Range(0.25f, 0.9f)));
    }

    private void Update()
    {
        if (!currentlyFishing)
            return;

        if (Input.GetKey(KeyCode.UpArrow))
            captureSlider.value += captureSliderSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            captureSlider.value -= captureSliderSpeed * Time.deltaTime;

        fishSlider.value += currentFishSpeed * Time.deltaTime;

        if (captureSlider.value >= fishSlider.value - 0.18f && captureSlider.value <= fishSlider.value + 0.18f)
        {
            chargeSlider.value += chargeSpeed * Time.deltaTime;
            if (chargeSlider.value >= 1)
            {
                GameManager.instance.AddItem(fishItem);
                EndFishing(true);
            }
        }
        else if (escapeSlider.value >= 1)
        {
            EndFishing(false);
        }

        escapeSlider.value += escapeSpeed * Time.deltaTime;
    }

    private IEnumerator FishMovement(float timer)
    {
        currentFishSpeed = Random.Range(fishMinSpeed,fishMaxSpeed);
        int dir = 1;
        if (fishSlider.value > 0.75)
            dir = 0;
        else if (fishSlider.value < 0.25)
            dir = 1;
        else
        {
            dir = Random.Range(0, 2);
        }
        if (dir == 0)
            currentFishSpeed *= -1;
        yield return new WaitForSeconds(timer);

        StartCoroutine(FishMovement(Random.Range(0.01f, 0.65f)));
        
    }

    private void EndFishing(bool catchFish)
    {
        StopAllCoroutines();
        fishingCanvas.SetActive(false);
        fisher.EndFishing();
        GameObject text = Instantiate(textPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        
        if (catchFish)
            GameManager.instance.BeginDialogue(dialogue[0]);
        else
            GameManager.instance.BeginDialogue(dialogue[1]);
        Destroy(text, 3f);

        currentlyFishing = false;
        StartCoroutine(InteractCooldown());
    }

    public override void BeginDialogue(PartyObject player)
    {
        fisher = player;
        fisher.BeginFishing();
        BeginFishing();
    }
}
