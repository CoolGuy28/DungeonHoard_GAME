using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenu : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    [SerializeField] private InventoryPanel invPanel;
    [SerializeField] private MainUIPanel mainPanel;
    private int currentMenuButton;
    [SerializeField] private string state = "Menu";
    private Item_Usable selectItem;
    private CharacterData characterData;
    private void Start()
    {
        foreach (var button in menuButtons)
            button.DeselectButton();
        menuButtons[currentMenuButton].SelectButton();
        if (currentMenuButton == 0)
        {
            mainPanel.gameObject.SetActive(true);
            invPanel.gameObject.SetActive(false);
        }
        else if (currentMenuButton == 1)
        {
            mainPanel.gameObject.SetActive(false);
            invPanel.gameObject.SetActive(true);
            invPanel.UpdateInventory();
        }
        else
        {
            mainPanel.gameObject.SetActive(false);
            invPanel.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        MenuControl();
    }

    private void MenuControl()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (state)
            {
                case "Menu":
                    SwitchMenuButton(-1);
                    break;
                case "Inventory":
                    invPanel.SwitchSelected(-2);
                    break;
                case "SelectingPlayer" or "SelectingItem":
                    mainPanel.SelectNewChar(-1);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (state)
            {
                case "Menu":
                    SwitchMenuButton(1);
                    break;
                case "Inventory":
                    invPanel.SwitchSelected(2);
                    break;
                case "SelectingPlayer" or "SelectingItem":
                    mainPanel.SelectNewChar(1);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switch (state)
            {
                case "Inventory":
                    invPanel.SwitchSelected(-1);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (state)
            {
                case "Inventory":
                    invPanel.SwitchSelected(1);
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (state)
            {
                case "Menu":
                    menuButtons[currentMenuButton].PressButton();
                    break;
                case "Main":
                    state = "SelectingPlayer";
                    break;
                case "Inventory":
                    Item_Usable selectedItem = invPanel.GetSelectedItem() as Item_Usable;
                    if (selectedItem != null)
                    {
                        if (selectedItem.GetAction().damageStats.damageType == DamageType.Healing)
                            selectItem = selectedItem;
                        mainPanel.gameObject.SetActive(true);
                        invPanel.gameObject.SetActive(false);
                        state = "SelectingItem";
                    }
                    break;
                case "SelectingItem":
                    characterData = mainPanel.GetSelectCharUIObj();
                    if(selectItem != null && GameManager.instance.GetItemAmount(selectItem) > 0)
                    {
                        GameManager.instance.UseItem(selectItem);
                        int d = (int)(selectItem.GetAction().damageStats.damage * characterData.currentStats.healingEffect);
                        characterData.AdjustHealth(-d);
                        if (selectItem.GetAction().damageStats.applyConditions.Length != 0)
                        {
                            if (selectItem.GetAction().damageStats.applySingleRandCondition)
                            {
                                int rand = Random.Range(0, 100);
                                if (rand <= selectItem.GetAction().damageStats.conditionChance)
                                {
                                    List<ConditionStats> acceptedConditions = new List<ConditionStats>();
                                    foreach (ConditionStats con in selectItem.GetAction().damageStats.applyConditions)
                                    {
                                        if (characterData.FindCondition(con.condition) == false)
                                        {
                                            acceptedConditions.Add(con);
                                        }
                                    }
                                    if (acceptedConditions.Count > 0)
                                    {
                                        characterData.AddCondition(acceptedConditions[Random.Range(0, acceptedConditions.Count)], null);
                                    }
                                }
                            }
                            else
                            {
                                foreach (ConditionStats condition in selectItem.GetAction().damageStats.applyConditions)
                                {
                                    int rand = Random.Range(0, 100);
                                    if (rand <= selectItem.GetAction().damageStats.conditionChance)
                                    {
                                        characterData.AddCondition(new ConditionStats(condition.condition, condition.timeFrame, condition.level), null);
                                    }
                                }
                            }
                        }
                        if (selectItem.GetAction().damageStats.removeConditions.Length != 0)
                        {
                            foreach (Condition removal in selectItem.GetAction().damageStats.removeConditions)
                            {
                                characterData.RemoveCondition(removal);
                            }
                        }
                    }
                    mainPanel.AdjustUIValues();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            switch (state)
            {
                case "Menu":
                    invPanel.gameObject.SetActive(false);
                    GameManager.instance.CloseOverworldMenu();
                    break;
                case "Inventory":
                    invPanel.gameObject.SetActive(false);
                    state = "Menu";
                    break;
                case "Main":
                    mainPanel.gameObject.SetActive(false);
                    state = "Menu";
                    break;
                case "SelectingPlayer":
                    state = "Main";
                    break;
                case "SelectingItem":
                    mainPanel.gameObject.SetActive(false);
                    invPanel.gameObject.SetActive(true);
                    state = "Inventory";
                    break;
            }
        }
    }

    public void SwitchMenuButton(int dir)
    {
        menuButtons[currentMenuButton].DeselectButton();
        currentMenuButton += dir;
        if (currentMenuButton < 0)
            currentMenuButton = menuButtons.Length - 1;
        else if (currentMenuButton >= menuButtons.Length)
            currentMenuButton = 0;
        menuButtons[currentMenuButton].SelectButton();
        if (currentMenuButton == 0)
        {
            mainPanel.gameObject.SetActive(true);
            invPanel.gameObject.SetActive(false);
        }
        else if (currentMenuButton == 1)
        {
            mainPanel.gameObject.SetActive(false);
            invPanel.gameObject.SetActive(true);
            invPanel.UpdateInventory();
        }
        else
        {
            mainPanel.gameObject.SetActive(false);
            invPanel.gameObject.SetActive(false);
        }
    }

    public void SetState (string state)
    {
        this.state = state;
    }
}
