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
    [SerializeField] private GameObject _sensitivityCanvasGO;
    [SerializeField] private GameObject _audioCanvasGO;
    
    [Header("Player Scripts to Deactivate on Pause")]
    public PlayerInput _playerInput;

    [Header("First Selected Options")] 
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _keyboardFirst;
    [SerializeField] private GameObject _gamepadFirst;
    [SerializeField] private GameObject _sensitivityFirst;
    [SerializeField] private GameObject _promptFirst;
    [SerializeField] private GameObject _audioFirst; 
    
    public InputActionAsset actions;
    
    private bool isPaused;
    private GameObject lastSelected;

    public PlayerMovement player;

    private void Start()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
        UnPause();
        CloseAllMenus();
    }

    private void Update()
    {
        if (player.MenuOpenCloseInput & _keyboardCanvasGO.activeSelf == false & _gamepadCanvasGO.activeSelf == false)
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
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        _settingsMenuCanvasGO.SetActive(true);
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    private void OpenKeyboardCanvas()
    {
        _keyboardCanvasGO.SetActive(true);
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_keyboardFirst);
    }

    private void OpenGamepadCanvas()
    {
        _gamepadCanvasGO.SetActive(true);
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_gamepadFirst);
    }

    private void OpenPrompt()
    {
        _promptCanvasGO.SetActive(true);
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_promptFirst);
    }
    
    private void OpenSensitivityCanvas()
    {
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(true);
        _promptCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_sensitivityFirst);
    }
    
    private void OpenAudioCanvas()
    {
        _audioCanvasGO.SetActive(true);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(_audioFirst); // Set the first selectable item in Audio Canvas
    }

    private void CloseAllMenus()
    {
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
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
    
    public void OnSensitivityPress()
    {
        OpenSensitivityCanvas();
    }
    
    public void OnAudioPress() // Add a new method for opening Audio Canvas
    {
        OpenAudioCanvas();
    }
    
    public void OnConfirmReturnToMainMenu()
    {
        // DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("MainMenu");
    }
    
    #endregion
    
    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }
        
    #endregion
    
    // Function to reload the current scene
    public void ReloadScene()
    {
        // Get the current active scene
        Scene currentScene = SceneManager.GetActiveScene();
        // Reload the current scene
        SceneManager.LoadScene(currentScene.name);
    }

    // Function to quit the game
    public void QuitGame()
    {
#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        // If running in a build, quit the application
        Application.Quit();
    }
}
