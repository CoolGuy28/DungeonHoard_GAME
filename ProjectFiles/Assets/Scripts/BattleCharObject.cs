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
        spriteObj.sprite = character.unit.battleSprite;
        spriteObj.sortingOrder = spritePriority;
        this.character = character;
        if (character.downed)
        {
            transform.rotation = Quaternion.Euler(transform.position.x, transform.position.y, -90);
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

    public void TakeDamage(int damage, Color damageColor)
    {
        character.AdjustHealth(damage);
        UpdateUI();
        GameObject damageText = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damage.ToString();
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = damageColor;
        Destroy(damageText, 1.5f);
        if (character.downed)
        {
            spriteObj.gameObject.transform.rotation = Quaternion.Euler(transform.position.x, transform.position.y, -90);
        }
    }
    public void TakeDamage(int damage, Action action, Color damageColor)
    {
        character.AdjustHealth(damage);
        GameObject damageText = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = damage.ToString();
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = damageColor;
        Destroy(damageText, 1.5f);
        if (character.downed)
        {
            spriteObj.gameObject.transform.rotation = Quaternion.Euler(transform.position.x, transform.position.y, -90);
        }
        
        if (action.particleEffect != null)
        {
            Instantiate(action.particleEffect, transform.position, Quaternion.identity, transform);
        }
        if (action.applyConditions.Length != 0)
        {
            foreach (ConditionStats condition in action.applyConditions)
            {
                int rand = Random.Range(0, 100);
                if (rand <= action.conditionChance)
                {
                    character.AddCondition(new ConditionStats(condition.condition, condition.timeFrame, condition.level), this);
                    UpdateConditionUI();
                }
            }
        }
        if (action.removeConditions.Length != 0)
        {
            foreach (Condition removal in action.removeConditions)
            {
                character.RemoveCondition(removal);
            }
            UpdateConditionUI();
        }
        UpdateUI();
    }

    public void HealCharacter(int healAmount, Action action)
    {
        int heal = (int)(healAmount * character.currentStats.healingEffect);
        character.AdjustHealth(-heal);
        GameObject damageText = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = heal.ToString();
        damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
        Destroy(damageText, 1.5f);
        if (character.downed)
        {
            spriteObj.gameObject.transform.rotation = Quaternion.Euler(transform.position.x, transform.position.y, 0);
            character.downed = false;
        }
        if (action.removeConditions.Length != 0)
        {
            foreach (Condition removal in action.removeConditions)
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
        if (i == 0)
            spriteObj.sprite = character.unit.attackSprite;
        else if (i == 1)
            spriteObj.sprite = character.unit.damageSprite;
        else
            spriteObj.sprite = character.unit.battleSprite;

        yield return new WaitForSeconds(timeFrame);

        spriteObj.sprite = character.unit.battleSprite;
    }
}
