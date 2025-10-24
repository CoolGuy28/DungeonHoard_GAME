using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MenuButton[] menuButtons;
    private int currentMenuButton;
    private AudioSource audioSource;
    [SerializeField] private AudioClip navigateClip;
    [SerializeField] private AudioClip selectClip;
    private void Start()
    {
        foreach (var button in menuButtons)
            button.DeselectButton();
        menuButtons[currentMenuButton].SelectButton();
        audioSource = GetComponent<AudioSource>();
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
            PlayAudioClip(navigateClip);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SwitchMenuButton(1);
            PlayAudioClip(navigateClip);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            menuButtons[currentMenuButton].PressButton();
            PlayAudioClip(selectClip);
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
        if (GameManager.instance.saved)
        {
            GameManager.instance.ChangeGameScene(GameManager.instance.gameData.sceneIndex);
        }
        else
        {
            GameManager.instance.ChangeGameScene(GameManager.instance.gameData.sceneIndex);
            GameManager.instance.NewGame();
        }
        
    }

    public void NewGame()
    {
        
        GameManager.instance.ChangeGameScene(3);
        GameManager.instance.NewGame();
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

}
