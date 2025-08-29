using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    private int currentMenuButton;
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

    public void ChangeScene()
    {
        GameManager.instance.ChangeGameScene(1);
    }

    public void NewGame()
    {
        GameManager.instance.NewGame();
        GameManager.instance.ChangeGameScene(1);
    }
}
