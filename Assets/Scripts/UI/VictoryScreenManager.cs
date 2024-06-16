using Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{
    public class VictoryScreenManager : MonoBehaviour
    {
        [Header("Menu Objects")] [SerializeField]
        private GameObject victoryScreenCanvasGo;

        [SerializeField] private GameObject videoPlayer;

        [Header("Misc")] [SerializeField] private AudioManager audioManager;


        [Header("First Selected Options")] [SerializeField]
        private GameObject victoryScreenFirst;

        public void OpenVictoryScreen()
        {
            victoryScreenCanvasGo.SetActive(true);
            videoPlayer.SetActive(true);
            InputManager.instance.DisableAllInputs();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            audioManager.PauseAudio();
            EventSystem.current.SetSelectedGameObject(victoryScreenFirst);
        }

        public void OnReturnToMainMenu()
        {
            // DataPersistenceManager.instance.SaveGame();
            SceneManager.LoadSceneAsync("MainMenu");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            // If running in a build, quit the application
            Application.Quit();
        }
    }
}