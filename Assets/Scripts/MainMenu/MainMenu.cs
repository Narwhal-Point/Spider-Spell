using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Menu Navigation")] [SerializeField]
    private SaveSlotsMenu saveSlotsMenu;
    
    [Header("Menu Buttons")] 
    
    [SerializeField] private Button newGameButton;
    
    [SerializeField] private Button continueGameButton;

    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;

        }
    }


    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
        this.DeactivateMenu();
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
        this.DeactivateMenu();
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        //save the game before loading a new scene
        // DataPersistenceManager.instance.SaveGame();
        // load the next scene - which will in turn load the game because of OnSceneLoaded() in the DataPersistenceManager
        SceneManager.LoadSceneAsync("SampleScene");
    }

    private void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }

    public void ActivateMenu()
    {
        this.GameObject().SetActive(true);
    }

    public void DeactivateMenu()
    {
        this.GameObject().SetActive(false);
    }
    
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        // If running in a build, quit the application
        Application.Quit();
    }
}
