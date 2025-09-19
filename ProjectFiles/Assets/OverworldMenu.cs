using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMenu : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    [SerializeField] private GameObject invPanel;
    private int currentMenuButton;
    private GameObject openedMenu;

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
        if (openedMenu == null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SwitchMenuButton(-1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SwitchMenuButton(1);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                menuButtons[currentMenuButton].PressButton();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (openedMenu != null)
            {
                openedMenu.SetActive(false);
                openedMenu = null;
            }
            else
            {
                invPanel.SetActive(false);
                GameManager.instance.CloseOverworldMenu();
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

    public void OpenMenu(GameObject menuObj)
    {
        menuObj.SetActive(true);
        openedMenu = menuObj;
    }
}
