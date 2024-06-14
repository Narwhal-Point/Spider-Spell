using System;
using System.Collections;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneController : MonoBehaviour, IDataPersistence
{
    public VideoPlayer videoPlayer;
    public bool shouldSkipIntroScene;
    public string sceneName = "SampleScene";

    private AsyncOperation asyncLoad;

    // objects for the skip option
    [SerializeField] private GameObject textObject;
    [SerializeField] private SetTextToTextBox text;
    [SerializeField] private PlayerInput _playerInput;
    private Coroutine _skipButtonCoroutine;

    public void Awake()
    {
        DataPersistenceManager.instance.LoadGame();
    }

    public void LoadData(GameData data)
    {
        shouldSkipIntroScene = data.shouldSkipIntro;
    }

    public void SaveData(GameData data)
    {
        data.shouldSkipIntro = shouldSkipIntroScene;
    }

    void Start()
    {
        textObject.SetActive(false);
        // Preload the scene asynchronously
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        shouldSkipIntroScene = false;
        // Check the boolean value
        if (shouldSkipIntroScene)
        {
            // If true, immediately switch to the preloaded scene
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            // Attach the event listener for when the video finishes playing
            videoPlayer.loopPointReached += OnVideoFinished;
            // If false, play the video
            videoPlayer.Play();
        }
    }

    private void Update()
    {
        if (CheckAnyButtonPress() && _skipButtonCoroutine == null)
        {
            _skipButtonCoroutine = StartCoroutine(ShowInteractButton());
        }
        else if (_skipButtonCoroutine != null && _playerInput.actions["Interact"].WasPerformedThisFrame())
        {
            asyncLoad.allowSceneActivation = true;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Set the boolean to true when the video has finished playing
        shouldSkipIntroScene = true;
        DataPersistenceManager.instance.SaveGame();

        // Immediately switch to the preloaded scene
        asyncLoad.allowSceneActivation = true;
    }

    void OnDestroy()
    {
        // Clean up the event listener when the script is destroyed
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    // function to check if any key at all is pressed.
    bool CheckAnyButtonPress()
    {
        // Check keyboard inputs
        if (Keyboard.current != null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }
        }
        
        // Check mouse inputs
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame || Mouse.current.backButton.wasPressedThisFrame ||
                Mouse.current.forwardButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        // Check gamepad inputs
        if (Gamepad.current != null)
        {
            var gamepad = Gamepad.current;
        
            if (gamepad.allControls.Any(control => control is ButtonControl button && button.wasPressedThisFrame))
            {
                return true;
            }
        }

        // Check mouse inputs
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
            {
                return true;
            }
        }
        // no button was pressed
        return false;
    }

    // display the interact button
    IEnumerator ShowInteractButton()
    {
        textObject.SetActive(true);   
        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            text.SetText("Press [Interact] to skip");
            yield return null;
        }
        textObject.SetActive(false);
        _skipButtonCoroutine = null;
    }
}