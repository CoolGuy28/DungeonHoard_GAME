using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    private int currentMenuButton;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameManager gameManager_Hard;
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
        GameManager.instance.LoadGame(gameManager);
    }

    public void ChangeSceneHard()
    {
        GameManager.instance.LoadGame(gameManager_Hard);
    }
}
