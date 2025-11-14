using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharObject : MonoBehaviour
{
    private CharacterData character;
    private SpriteRenderer spriteObj;
    private Animator animator;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject conditionUIPrefab;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject currentObIcon;
    //[SerializeField] private GameObject hiddenInfo;

    public void SetCharacterObject(CharacterData character, int spritePriority)
    {
        spriteObj = transform.GetChild(1).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(1).GetComponent<Animator>();
        spriteObj.sortingOrder = spritePriority;
        spriteObj.gameObject.transform.localScale = character.unit.spriteSize;
        this.character = character;
        battleSprite = character.unit.battleSprite;
        basicAttack = character.unit.basicAttack;
        attackSprites = character.unit.attackSprites;
        damageSprite = character.unit.damageSprite;
        downedSprite = character.unit.downedSprite;
        if (character.downed)
        {
            spriteObj.sprite = downedSprite;
        }
        else
        {
            spriteObj.sprite = battleSprite;
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
    public void TakeDamage(DamageStats damageStats, bool crit, AudioClip attackSFX)
    {
        int damageAmount = Random.Range(damageStats.damage - damageStats.randomVarience, damageStats.damage + damageStats.randomVarience);
        if (damageAmount < 0)
            damageAmount = 0;
        GameObject damageText = CreateText();
        if (crit)
            damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = "CRIT\n";
        switch (damageStats.damageType)
        {
            case DamageType.Physical:
                damageAmount = (int)(damageAmount * character.currentStats.defence);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
            case DamageType.Magic:
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
            case DamageType.Healing:
                damageAmount = (int)(damageAmount * character.currentStats.healingEffect);
                character.AdjustHealth(-damageAmount);
                if (damageAmount > 0)
                    damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                else
                    damageText.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                if (!character.downed)
                {
                    spriteObj.sprite = battleSprite;
                }
                break;
            case DamageType.Fire:
                damageAmount = (int)(damageAmount * character.currentStats.fireRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
            case DamageType.Poison:
                damageAmount = (int)(damageAmount * character.currentStats.poisonRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.magenta;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
            case DamageType.Cold:
                damageAmount = (int)(damageAmount * character.currentStats.coldRes);
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.blue;
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
            default:
                character.AdjustHealth(damageAmount);
                damageText.transform.GetChild(0).GetComponent<TMP_Text>().text += damageAmount.ToString();
                break;
        }
        
        if (character.downed)
        {
            spriteObj.sprite = downedSprite;
        }
        
        if (damageStats.particleEffect != null)
        {
            Instantiate(damageStats.particleEffect, transform.position, Quaternion.identity, transform);
        }

        if (attackSFX != null)
            PlayAudioClip(attackSFX);

        if (damageStats.applyConditions.Length != 0)
        {
            if (damageStats.applySingleRandCondition)
            {
                int rand = Random.Range(0, 100);
                if (rand <= damageStats.conditionChance)
                {
                    List<ConditionStats> acceptedConditions = new List<ConditionStats>();
                    foreach (ConditionStats con in damageStats.applyConditions)
                    {
                        if (GetCharacter().FindCondition(con.condition) == false)
                        {
                            acceptedConditions.Add(con);
                        }
                    }
                    if (acceptedConditions.Count > 0)
                    {
                        AddCondition(acceptedConditions[Random.Range(0, acceptedConditions.Count)]);
                    }
                }
            }
            else
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

    public void AttackMissed(AudioClip attackSFX)
    {
        GameObject missText = CreateText();
        missText.transform.GetChild(0).GetComponent<TMP_Text>().text = "Miss";
        if (attackSFX != null)
            PlayAudioClip(attackSFX);
    }

    private GameObject CreateText()
    {
        GameObject text = Instantiate(damageTextPrefab, new Vector3(Random.Range(transform.position.x - 1, transform.position.x + 1), Random.Range(transform.position.y - 1, transform.position.y + 1), transform.position.z), Quaternion.identity);
        Destroy(text, 1.5f);
        return text;
    }

    public void AddCondition(ConditionStats conditionStats)
    {
        character.AddCondition(conditionStats, this);
        UpdateConditionUI();
    }

    public void TickConditions()
    {
        if (character.conditions != null)
        {
            for (int i = 0; i < character.conditions.Count; i++)
            {
                if (character.conditions[i].timeFrame != -1)
                {
                    character.conditions[i].timeFrame--;
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
            if (condition.timeFrame > 0)
            {
                newConditionUI.transform.GetChild(0).GetComponent<TMP_Text>().text = condition.timeFrame.ToString();
            }
            else
            {
                newConditionUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }
        }
    }

    /*public void DisplayHiddenInfo()
    {
        hiddenInfo.SetActive(true);
        Stats stats = character.currentStats;
        string statsText = "Attack - " + stats.attack + "\nDefence - " + stats.defence + "\nMaxHp - " + stats.maxHealth + "\nHealing - " + stats.healingEffect + "\nFireRes - " + stats.fireRes + "\nColdRes - " + stats.coldRes +
            "\nPoiRes - " + stats.poisonRes + "\nSpeed - " + stats.speed + "\nAccuracy - " + stats.accuracy + "\nCrit% - " + stats.critPercent + "\nCritMult - " + stats.critMultiplyer + "\nActions - " + stats.actions;
        hiddenInfo.transform.GetChild(1).GetComponent<TMP_Text>().text = statsText;
        string condText = "";
        if (character.conditions.Count > 0)
        {
            condText = "Conditions";
            foreach (ConditionStats condition in character.conditions)
            {
                condText += "\n" + condition.condition.name;
            }
        }
        hiddenInfo.transform.GetChild(2).GetComponent<TMP_Text>().text = condText;
    }

    public void HideHiddenInfo()
    {
        hiddenInfo.SetActive(false);
    }*/

    public void BossTransformation()
    {
        Unit_Boss bossUnit = character.unit as Unit_Boss;
        character.downed = false;
        character.baseStats = new Stats(bossUnit.baseStats_Phase2);
        character.currentStats = new Stats(bossUnit.baseStats_Phase2);
        character.weapon = bossUnit.weapon_Phase2;
        character.skills = bossUnit.skills_Phase2;
        character.currentHealth = bossUnit.baseStats_Phase2.maxHealth;
        character.SetCurrentStats();
        battleSprite = bossUnit.battleSprite_Phase2;
        basicAttack = bossUnit.basicAttack_Phase2;
        attackSprites = bossUnit.attackSprites_Phase2;
        damageSprite = bossUnit.damageSprite_Phase2;
        downedSprite = bossUnit.downedSprite_Phase2;
        PlayAudioClip(bossUnit.transformSound);
    }

    public void UpdateUI()
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

    public void SelectAsCurrentControl()
    {
        currentObIcon.SetActive(true);
    }

    public void DeselectAsCurrentControl()
    {
        currentObIcon.SetActive(false);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public Sprite battleSprite;
    public Sprite basicAttack;
    public Sprite[] attackSprites;
    public Sprite damageSprite;
    public Sprite downedSprite;
    public IEnumerator SetSprite(int i, float timeFrame, int attackIndex)
    {
        if (!character.downed)
        {
            if (i == 0)
            {
                spriteObj.sprite = attackSprites[attackIndex];
                animator.Play("Attack");
            }
            else if (i == 1)
            {
                spriteObj.sprite = damageSprite;
                animator.Play("Damaged");
            }
            else
                spriteObj.sprite = battleSprite;

            yield return new WaitForSeconds(timeFrame);

            if (character.downed)
            {
                spriteObj.sprite = downedSprite;
            }
            else
            {
                spriteObj.sprite = battleSprite;
            }
        }
    }
}
