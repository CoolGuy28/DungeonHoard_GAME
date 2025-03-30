using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubMenuButton : MonoBehaviour
{
    private Ability ability;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SelectButton()
    {
        image.color = activeColor;
    }

    public void DeselectButton()
    {
        image.color = inactiveColor;
    }

    public void SetSkill(Ability ability)
    {
        this.ability = ability;
        nameText.text = ability.name;
        if (ability is Spell)
        {
            Spell spell = ability as Spell;
            costText.text = spell.staminaCost.ToString();
        }
        else if (ability is Item)
        {
            Item item = ability as Item;
            costText.text = "x" + GameManager.instance.GetItemAmount(item).ToString();
        }
    }

    public Ability GetSkill()
    {
        return ability;
    }
}
