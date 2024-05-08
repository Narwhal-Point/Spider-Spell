using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class SetTextToTextBox : MonoBehaviour
    {

        [Header("Setup for sprites")] [SerializeField]
        private ButtonPromptsSpriteAssests buttonassets;

        [FormerlySerializedAs("_deviceType")] [SerializeField] private DeviceType deviceType;
        
        private PlayerInput _playerInput;
        private TMP_Text _textBox;

        private void Awake()
        {
            GameObject player = GameObject.Find("Player");
            _playerInput = player.GetComponent<PlayerInput>();
            _textBox = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            SetText("Press BUTTONPROMPT to do nothing","Sprint");
        }
        public void SetText(string message, string action)
        {
            string currentControlScheme = _playerInput.currentControlScheme;
            if (currentControlScheme == "Gamepad")
            {
                deviceType = DeviceType.Gamepad;
            }
            else if (currentControlScheme == "Keyboard&Mouse")
            {
                deviceType = DeviceType.Keyboard;
            }
            
            if ((int)deviceType > buttonassets.spriteAssets.Count - 1)
            {
                Debug.LogWarning($"Missing Sprite Asset for {deviceType}");
                return;
            }

            _textBox.text = CompleteTextWithButtonPromptSprite.ReadAndReplaceBinding(
                message, _playerInput.actions.FindAction(action).bindings[(int)deviceType], 
                buttonassets.spriteAssets[(int)deviceType]);
        }
        
        [ContextMenu("Set Text without action")]
        public void SetText(string message)
        {
            string currentControlScheme = _playerInput.currentControlScheme;
            if (currentControlScheme == "Gamepad")
            {
                deviceType = DeviceType.Gamepad;
            }
            else if (currentControlScheme == "Keyboard&Mouse")
            {
                deviceType = DeviceType.Keyboard;
            }
            
            if ((int)deviceType > buttonassets.spriteAssets.Count - 1)
            {
                Debug.LogWarning($"Missing Sprite Asset for {deviceType}");
                return;
            }

            _textBox.text = message;
        }

        // TODO: switch to reading which device is active
        private enum DeviceType
        {
            Gamepad = 0,
            Keyboard = 1
        }
    }
}