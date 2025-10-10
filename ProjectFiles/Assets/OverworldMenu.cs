using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenu : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    [SerializeField] private InventoryPanel invPanel;
    [SerializeField] private GameObject mainPanel;
    private int currentMenuButton;
    [SerializeField] private string state = "Menu";

    private void Start()
    {
        foreach (var button in menuButtons)
            button.DeselectButton();
        menuButtons[currentMenuButton].SelectButton();
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
                    mainPanel.SetActive(false);
                    state = "Menu";
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
    }

    public void SetState (string state)
    {
        this.state = state;
    }
}
