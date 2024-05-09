using System.Text.RegularExpressions;
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

        [FormerlySerializedAs("_deviceType")] [SerializeField]
        private DeviceType deviceType;

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
            SetText("Press [Sprint] to do nothing");
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

            // Find all matches for string between '[' and ']' 
            MatchCollection matches = Regex.Matches(message, @"\[(.*?)\]");
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string action = match.Groups[1].Value;
                    InputBinding binding = _playerInput.actions.FindAction(action).bindings[(int)deviceType];
                    TMP_SpriteAsset spriteAsset = buttonassets.spriteAssets[(int)deviceType];
                    string replacement = CompleteTextWithButtonPromptSprite.ReadAndReplaceBinding(match.Value, binding, spriteAsset);
                    message = message.Replace(match.Value, replacement);
                }
            }

            _textBox.text = message;
        }

        private enum DeviceType
        {
            Gamepad = 0,
            Keyboard = 1
        }
    }
}