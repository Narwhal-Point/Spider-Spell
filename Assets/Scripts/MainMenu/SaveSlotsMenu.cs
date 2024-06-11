using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")] [SerializeField]
    private MainMenu mainMenu;

    [Header("Menu Buttons")] 
    [SerializeField] private Button backButton;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        // disable all buttons
        DisableMenuButtons();
        
        // update the selected profile id to be used for data persistence
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (!isLoadingGame)
        {
            // create a new game which will initialize our data to a clean slate
            DataPersistenceManager.instance.NewGame();
        }
        
        //save the game anytime before loading a new scene
        DataPersistenceManager.instance.SaveGame();

        // load the scene - which will in turn save the game because of OnSceneUnloaded in the DataPersistenceManager
        SceneManager.LoadSceneAsync("IntroScene");
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }
    

    public void ActivateMenu(bool isLoadingGame)
    {
        // set this menu to be active
        this.gameObject.SetActive(true);
        
        //set mode
        this.isLoadingGame = isLoadingGame;
        
        // Load all of the profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();
        
        // loop through each save slot in the UI and set the content appropiately
        GameObject firstSelected = backButton.gameObject;
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
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }

        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        this.SetFirstSelected(firstSelectedButton);
    }

    public void DeactivateMenu()
    {
        this.GameObject().SetActive(false);
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
