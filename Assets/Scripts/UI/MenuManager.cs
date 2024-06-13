using System.Collections;
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
        if (_cachedControlScheme != null)
        {
            _playerInput.SwitchCurrentControlScheme(_cachedControlScheme);
        }
    }
    
    #endregion
    
    private void Start()
    {
        _cachedControlScheme = _playerInput.currentControlScheme;
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
        UnPause();
        CloseAllMenus();
    }

    private void Update()
    {
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
        
        else if (cancelAction & _keyboardCanvasGO.activeSelf == true & _waitForInputKeyboard.activeSelf ==false)
        {
            cancelAction = false;
            OpenSettingsMenuHandle();
        }
        
        else if (cancelAction & _gamepadCanvasGO.activeSelf == true & _waitForInputGamePad.activeSelf ==false)
        {
            cancelAction = false;
            OpenSettingsMenuHandle();
        }
        
        else if (cancelAction & _mainMenuCanvasGO.activeSelf == true)
        {
            cancelAction = false;
            UnPause();
        }
    }

    #region Pause/Unpause Functions

    public void Pause()
    {
        OpenMainMenu();
        InputManager.instance.DisableAllInputs();
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

        StartCoroutine(DelayInputEnable());
        CloseAllMenus();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator DelayInputEnable()
    {
        yield return new WaitForSeconds(0.1f);
        InputManager.instance.EnableAllInputs();
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
        if (Gamepad.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
        }
    }
}
