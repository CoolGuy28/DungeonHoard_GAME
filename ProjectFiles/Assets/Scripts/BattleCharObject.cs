using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharObject : MonoBehaviour
{
    private CharacterData character;
    private SpriteRenderer spriteObj;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject conditionUIPrefab;

    public void SetCharacterObject(CharacterData character, int spritePriority)
    {
        spriteObj = transform.GetChild(1).GetComponent<SpriteRenderer>();
        spriteObj.sortingOrder = spritePriority;
        spriteObj.gameObject.transform.localScale = character.unit.spriteSize;
        this.character = character;
        if (character.downed)
        {
            spriteObj.sprite = character.unit.downedSprite;
        }
        else
        {
            spriteObj.sprite = character.unit.battleSprite;
        }
        InitialiseUI();
    }

    private void InitialiseUI()
    {
        if (character != null)
        {
            ui.transform.GetChild(0).GetComponent<Slider>().maxValue = character.currentStats.maxHealth;
            ui.transform.GetChild(0).GetComponent<Slider>().value = character.currentHealth;
            UpdateConditionUI();
        }
    }
    public void TakeDamage(DamageStats damageStats)
    {
        int damageAmount = Random.Range(damageStats.damage - damageStats.randomVarience, damageStats.damage + damageStats.randomVarience);
        if (damageAmount < 0)
            damageAmount = 0;
        GameObject damageText = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        switch (damageStats.damageType)
        {
            case DamageType.Physical:
                damageAmount = (int)(damageAmount * character.currentStats.defence);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
            case DamageType.Magic:
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
            case DamageType.Healing:
                damageAmount = (int)(damageAmount * character.currentStats.healingEffect);
                character.AdjustHealth(-damageAmount);
                if (damageAmount > 0)
                    damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                else
                    damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                if (!character.downed)
                {
                    spriteObj.sprite = character.unit.battleSprite;
                }
                break;
            case DamageType.Fire:
                damageAmount = (int)(damageAmount * character.currentStats.fireRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
            case DamageType.Poison:
                damageAmount = (int)(damageAmount * character.currentStats.poisonRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.magenta;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
            case DamageType.Cold:
                damageAmount = (int)(damageAmount * character.currentStats.coldRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.blue;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
            default:
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damageAmount.ToString();
                break;
        }
        Destroy(damageText, 1.5f);
        if (character.downed)
        {
            spriteObj.sprite = character.unit.downedSprite;
        }
        
        if (damageStats.particleEffect != null)
        {
            Instantiate(damageStats.particleEffect, transform.position, Quaternion.identity, transform);
        }
        if (damageStats.applyConditions.Length != 0)
        {
            foreach (ConditionStats condition in damageStats.applyConditions)
            {
                int rand = Random.Range(0, 100);
                if (rand <= damageStats.conditionChance)
                {
                    character.AddCondition(new ConditionStats(condition.condition, condition.timeFrame, condition.level), this);
                    UpdateConditionUI();
                }
            }
        }
        if (damageStats.removeConditions.Length != 0)
        {
            foreach (Condition removal in damageStats.removeConditions)
            {
                character.RemoveCondition(removal);
            }
            UpdateConditionUI();
        }
        UpdateUI();
    }

    public void TickConditions()
    {
        if (character.conditions != null)
        {
            for (int i = 0; i < character.conditions.Count; i++)
            {
                character.conditions[i].timeFrame--;
                if (character.conditions[i].timeFrame != -1)
                {
                    if (character.conditions[i].timeFrame == 0)
                    {
                        character.conditions[i].condition.OnConditionEnd(this, character.conditions[i].level);
                    }
                    else
                    {
                        character.conditions[i].condition.OnConditionTick(this, character.conditions[i].level);
                    }
                }
                else
                {
                    character.conditions[i].condition.OnConditionTick(this, character.conditions[i].level);
                }
            }
            UpdateConditionUI();
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
            GameObject newConditionUI = Instantiate(conditionUIPrefab, Vector3.zero, Quaternion.identity, ui.transform.GetChild(1));
            if (condition.condition.sprite != null)
            {
                newConditionUI.GetComponent<Image>().sprite = condition.condition.GetSprite(condition.level);
            }
            newConditionUI.transform.GetChild(0).GetComponent<TMP_Text>().text = condition.timeFrame.ToString();
        }
    }

    private void UpdateUI()
    {
        ui.transform.GetChild(0).GetComponent<Slider>().value = character.currentHealth;
    }

    public CharacterData GetCharacter()
    {
        return character;
    }

    public void SetWhite()
    {
        spriteObj.color = Color.white;
    }

    public void SetYellow()
    {
        spriteObj.color = Color.yellow;
    }

    public void SetDeselected()
    {
        spriteObj.color = new Color(0.2f, 0.2f, 0.2f);
    }

    public IEnumerator SetSprite(int i, float timeFrame)
    {
        if (!character.downed)
        {
            if (i == 0)
                spriteObj.sprite = character.unit.attackSprite;
            else if (i == 1)
                spriteObj.sprite = character.unit.damageSprite;
            else
                spriteObj.sprite = character.unit.battleSprite;

            yield return new WaitForSeconds(timeFrame);

            if (character.downed)
            {
                spriteObj.sprite = character.unit.downedSprite;
            }
            else
            {
                spriteObj.sprite = character.unit.battleSprite;
            }
        }
    }
}
