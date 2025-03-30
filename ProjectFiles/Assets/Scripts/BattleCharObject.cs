using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharObject : MonoBehaviour
{
    private BaseCharacter character;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject damageTextPrefab;

    public void SetCharacterObject(BaseCharacter character)
    {
        GetComponent<SpriteRenderer>().sprite = character.battleSprite;
        this.character = character;
    }

    public void InitialiseUI()
    {
        if (character != null)
        {
            ui.transform.GetChild(0).GetComponent<Slider>().maxValue = character.baseStats.maxHealth;
            ui.transform.GetChild(0).GetComponent<Slider>().value = character.currentHealth;
        }
    }

    public void TakeDamage(int damage, Action action)
    {
        character.TakeDamage(damage);
        UpdateUI();
        GameObject damageText = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damage.ToString();
        Destroy(damageText, 1.5f);
        if (character.downed)
        {
            transform.rotation = Quaternion.Euler(transform.position.x, transform.position.y, -90);
        }
        if (action.particleEffect != null)
        {
            Instantiate(action.particleEffect, transform.position, Quaternion.identity);
        }
        if (action.applyCondition != null)
        {
            foreach (ConditionStats condition in action.applyCondition)
            {
                int rand = Random.Range(0, 100);
                if (rand <= action.conditionChance)
                {
                    character.AddCondition(condition);
                    UpdateConditionUI();
                }
            }
        }
    }

    private void UpdateConditionUI()
    {
        foreach (Transform child in ui.transform.GetChild(1))
        {
            Destroy(child.gameObject);
        }
        foreach (ConditionStats condition in character.conditions)
        {
            GameObject newConditionUI = new GameObject();
            newConditionUI.AddComponent<Image>();
            newConditionUI.transform.SetParent(ui.transform.GetChild(1));
            newConditionUI.transform.localScale = Vector3.one;
            if (condition.condition.sprite != null)
            {
                newConditionUI.GetComponent<Image>().sprite = condition.condition.sprite;
            }
        }
    }

    private void UpdateUI()
    {
        ui.transform.GetChild(0).GetComponent<Slider>().value = character.currentHealth;
    }

    public BaseCharacter GetCharacter()
    {
        return character;
    }

    public void SetWhite()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetYellow()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public void SetDeselected()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);
    }
}
