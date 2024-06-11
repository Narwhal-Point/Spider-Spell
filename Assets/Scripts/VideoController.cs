using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneController : MonoBehaviour, IDataPersistence
{
    public VideoPlayer videoPlayer;
    public bool shouldSkipIntroScene;
    public string sceneName = "SampleScene";

    private AsyncOperation asyncLoad;

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
        // Preload the scene asynchronously
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

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
}