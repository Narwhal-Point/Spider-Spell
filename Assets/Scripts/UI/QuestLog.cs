using Audio;
using UnityEngine;
using UnityEngine.InputSystem;


namespace UI
{
    public class QuestLog : MonoBehaviour, IDataPersistence
    {
        // input
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string closingMessage = "Press [QuestLog] to Close";
        private InputAction _logAction;
        private AudioManager _audioManager;

        // image gameobject to enable / disable
        private GameObject _logImage;
        private SetTextToTextBox _closeText;

        // current control scheme and device
        // private string _currentControlScheme;
        // private InputDevice _currentInputDevice;


        private void Awake()
        {
            // these actions need to be called in awake, because if they're called in start they don't get assigned.
            // assign the input action
            _logAction = playerInput.actions["QuestLog"];
            // get the image and disable it
            _logImage = transform.GetChild(0).gameObject;
            _logImage.SetActive(false);
            _closeText = transform.GetChild(0).GetChild(0).GetComponent<SetTextToTextBox>();

            _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }

        private void Update()
        {
            if (InputManager.instance.QuestLogInput)
            {
                if (_logImage.activeSelf)
                {
                    CloseQuestLog();
                }
                else
                {
                    OpenQuestLog();
                }
            }
        }

        public void OpenQuestLog()
        {
            _closeText.SetText(closingMessage);
            
            // needs to play when timescale is 1.
            _audioManager.PlaySFX(_audioManager.paperSfx);
            
            // save the current input device before disabling player input
            // _currentControlScheme = playerInput.currentControlScheme;
            // _currentInputDevice = playerInput.devices[0];
            InputManager.instance.DisableAllInputsButMenu();
            // playerInput.enabled = false;
            // enable this action again because otherwise the log will never close
            _logAction.Enable();
            // enable the gameobject
            _logImage.SetActive(true);
            // pause the game
            Time.timeScale = 0f;
        }

        private void CloseQuestLog()
        {
            // playerInput.enabled = true;
            InputManager.instance.EnableAllInputs();
            // set the control scheme back. This is only really an issue when using a controller
            // and you're checking the quest log while there's a hud text with a button prompt.
            // Enabling player input defaults to keyboard&Mouse, so the keyboard/mouse icon will be shown.
            // playerInput.SwitchCurrentControlScheme(_currentControlScheme, _currentInputDevice);
            // disable the gameobject
            _logImage.SetActive(false);
            // resume the game
            Time.timeScale = 1f;
            
            // needs to play when timescale is 1.
            _audioManager.PlaySFX(_audioManager.paperSfx);
        }

        // Make the journal collected value persistant
        public void LoadData(GameData data)
        {
            gameObject.SetActive(data.journalCollected);
        }

        public void SaveData(GameData data)
        {
            data.journalCollected = gameObject.activeSelf;
        }
    }
}