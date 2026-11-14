using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 


public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject selectLevelPanel;

    [SerializeField] private int LevelID;

    enum UIState
    {
        Option,
        Credit,
        Exit,
        Neutral,
    }

    UIState UIStates;

    void Start() { UIStates = UIState.Neutral; }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (UIStates == UIState.Option)
            {
                optionPanel.SetActive(false);
                UIStates = UIState.Neutral;
            }
            else if (UIStates == UIState.Credit)
            {
                creditPanel.SetActive(false);
                UIStates = UIState.Neutral;
            }
            else if (UIStates == UIState.Exit)
            {
                exitPanel.SetActive(false);
                UIStates = UIState.Neutral;
            }
        }
    }

    void NextLevel()
    {
        int nextLevel = LevelID + 1;
        string nextLevelText = $"Level {nextLevel}";
        SceneManager.LoadScene(nextLevelText);
    }

    void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Option()
    {
        optionPanel.SetActive(true);
        UIStates = UIState.Option; 
    }

    void Exit()
    {
        exitPanel.SetActive(true);
        UIStates = UIState.Exit;
    }

    void Play()
    {
        //selectLevelPanel.SetActive(true);
        // SceneManager.LoadScene("Select Level");

        // yang mana nih???????
        // ai jangan anu ini dulu
    }
}