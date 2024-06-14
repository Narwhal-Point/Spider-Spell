using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")] 
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Buttons")] 
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;
    
    [SerializeField] private InputSystemUIInputModule iptmod;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    private void Update()
    {
        bool cancelAction = iptmod.cancel.action.WasPerformedThisFrame();

        if (cancelAction)
        {
            OnBackClicked();
        }
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // Disable all buttons
        DisableMenuButtons();
        
        // Update the selected profile id to be used for data persistence
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (!isLoadingGame)
        {
            // Create a new game which will initialize our data to a clean slate
            DataPersistenceManager.instance.NewGame();
        }
        
        // Save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        // Load the scene - which will in turn save the game because of OnSceneUnloaded in the DataPersistenceManager
        SceneManager.LoadSceneAsync("IntroScene");
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }
    
    public void ActivateMenu(bool isLoadingGame)
    {
        // Set this menu to be active
        this.gameObject.SetActive(true);
        
        // Set mode
        this.isLoadingGame = isLoadingGame;
        
        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();
        
        // Loop through each save slot in the UI and set the content appropriately
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }

        backButton.interactable = false;
    }
}
