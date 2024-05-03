using System.Collections;
using Player.Movement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;
    [SerializeField] private GameObject _keyboardCanvasGO;
    [SerializeField] private GameObject _gamepadCanvasGO;
    [SerializeField] private GameObject _promptCanvasGO;
    
    [Header("Player Scripts to Deactivate on Pause")]
    public PlayerInput _playerInput;

    [Header("First Selected Options")] 
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _keyboardFirst;
    [SerializeField] private GameObject _gamepadFirst;
    [SerializeField] private GameObject _promptFirst;
    
    private bool isPaused;
    private GameObject lastSelected;

    public PlayerMovement player;

    private void Start()
    {
        UnPause();
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false); 
    }

    private void Update()
    {
        if (player.MenuOpenCloseInput)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }
    
    #region Pause/Unpause Functions

    public void Pause()
    {
        OpenMainMenu();
        _playerInput.enabled = false;
        _playerInput.actions["MenuOpenClose"].Enable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        CloseAllMenus();
        _playerInput.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    #endregion
    
    #region Canvas Activations/Deactivations

    private void OpenMainMenu()
    {
        _mainMenuCanvasGO.SetActive(true);
        _promptCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        _settingsMenuCanvasGO.SetActive(true);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    private void OpenKeyboardCanvas()
    {
        _keyboardCanvasGO.SetActive(true);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_keyboardFirst);
    }

    private void OpenGamepadCanvas()
    {
        _gamepadCanvasGO.SetActive(true);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_gamepadFirst);
    }

    private void OpenPrompt()
    {
        _promptCanvasGO.SetActive(true);
        _gamepadCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_promptFirst);
    }

    private void CloseAllMenus()
    {
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    #endregion
    
    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnKeyboardPress()
    {
        OpenKeyboardCanvas();
    }

    public void OnGamepadPress()
    {
        OpenGamepadCanvas();
    }

    public void OnBackPressRebind()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        UnPause();
    }

    public void OnReturnToMainMenuPress()
    {
        OpenPrompt();
    }
    
    public void OnConfirmReturnToMainMenu()
    {
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    #endregion
    
    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }
        
    #endregion
}
