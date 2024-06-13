using Audio;
using Player.Movement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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
    [SerializeField] private GameObject _waitForInputKeyboard;
    [SerializeField] private GameObject _waitForInputGamePad;
    
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
    
    [Header("Misc")]
    
    [SerializeField] private InputActionAsset actions;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private InputSystemUIInputModule iptmod;
    private bool _isPaused;
    
    private InputAction _navigate;
    private InputAction _point;
    private bool _usingNavigateAction;

    // fix for inputs defaulting to keyboard after closing menu
    #region Input Caching
    
    private string _cachedControlScheme;

    private void CacheControlScheme()
    {
        _cachedControlScheme = _playerInput.currentControlScheme;
    }

    private void SetControlScheme()
    {
        _playerInput.enabled = true;
       _playerInput.SwitchCurrentControlScheme(_cachedControlScheme);
    }
    
    #endregion
    

private void Start()
{
    // setting up the input actions for hotswapping between keyboard and controller
    _navigate = _playerInput.actions["Navigate"];
    _point = _playerInput.actions["Point"];
    
    _cachedControlScheme = _playerInput.currentControlScheme;
    var rebinds = PlayerPrefs.GetString("rebinds");
    if (!string.IsNullOrEmpty(rebinds))
        actions.LoadBindingOverridesFromJson(rebinds);
    UnPause();
    CloseAllMenus();
}

private void Update()
{
    // check if either arrow keys/ gamepad controls are used or mouse is used
    if (_navigate.WasPerformedThisFrame())
        _usingNavigateAction = true;
    else if (_point.WasPerformedThisFrame())
        _usingNavigateAction = false;

    bool cancelAction = iptmod.cancel.action.WasPerformedThisFrame();
    if (player.MenuOpenCloseInput)
    {
        if (!_isPaused)
        {
            Pause();
        }
        // else
        // {
        //     UnPause();
        // }
    }

    else if (cancelAction & (_promptCanvasGO.activeSelf == true | _settingsMenuCanvasGO.activeSelf == true))
    {
        cancelAction = false;
        OpenMainMenu();
    }


    else if (cancelAction & (_sensitivityCanvasGO.activeSelf == true | _audioCanvasGO.activeSelf == true))
    {
        cancelAction = false;
        OpenSettingsMenuHandle();
    }

    else if (cancelAction & _keyboardCanvasGO.activeSelf == true & _waitForInputKeyboard.activeSelf == false)
    {
        cancelAction = false;
        OpenSettingsMenuHandle();
    }

    else if (cancelAction & _gamepadCanvasGO.activeSelf == true & _waitForInputGamePad.activeSelf == false)
    {
        cancelAction = false;
        OpenSettingsMenuHandle();
    }

    else if (cancelAction & _mainMenuCanvasGO.activeSelf == true)
    {
        cancelAction = false;
        UnPause();
    }

    // select the box if using navigate. Else don't select anything
    else if (!EventSystem.current.currentSelectedGameObject && _usingNavigateAction)
    {
        SetSelectedGameObjectIfGamepad(_mainMenuFirst);
    }
    else if (!_usingNavigateAction)
    {
        SetSelectedGameObjectIfGamepad(null);
    }
}

    #region Pause/Unpause Functions

    public void Pause()
    {
        OpenMainMenu();
        _playerInput.enabled = false;
        // _playerInput.actions["MenuOpenClose"].Enable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void UnPause()
    {
        _isPaused = false;
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
        CacheControlScheme();
        
        _mainMenuCanvasGO.SetActive(true);
        _audioCanvasGO.SetActive(false);
        _sensitivityCanvasGO.SetActive(false);
        _promptCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardCanvasGO.SetActive(false);
        _gamepadCanvasGO.SetActive(false);
        
        SetSelectedGameObjectIfGamepad(_mainMenuFirst);
        audioManager.PauseAudio();
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
        
        SetSelectedGameObjectIfGamepad(_settingsMenuFirst);
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
        
        SetSelectedGameObjectIfGamepad(_keyboardFirst);
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
        
        SetSelectedGameObjectIfGamepad(_gamepadFirst);
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
        
        SetSelectedGameObjectIfGamepad(_promptFirst);
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
        
        SetSelectedGameObjectIfGamepad(_sensitivityFirst);
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
        
        SetSelectedGameObjectIfGamepad(_audioFirst);
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
        audioManager.UnpauseAudio();
        
        SetControlScheme();
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
    
    private void SetSelectedGameObjectIfGamepad(GameObject gameObjectToSelect)
    {
        if (_usingNavigateAction)
        {
            EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

    }
}
