using System;
using System.Collections.Generic;
using System.Linq;
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
            else if (currentControlScheme == "playstation")
            {
                deviceType = DeviceType.Playstation;
            }
            else if (currentControlScheme == "Switch")
            {
                deviceType = DeviceType.Switch;
            }
            else if (currentControlScheme == "Keyboard&Mouse")
            {
                deviceType = DeviceType.Keyboard;
            }
            else
            {
                Debug.LogError($"Unexpected control scheme: {currentControlScheme}");
                return;
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
                    string actionName = match.Groups[1].Value;
                    
                    try
                    {
                        _playerInput.actions.FindAction(actionName, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"action {actionName} does not exist!");
                        continue;
                    }
                    
                    InputBinding binding = _playerInput.actions.FindAction(actionName).bindings[(int)deviceType];
                    TMP_SpriteAsset spriteAsset = buttonassets.spriteAssets[(int)deviceType];

                    if (binding.isComposite)
                    {
                        // add all bindings that are part of this binding to the list
                        List<InputBinding> compositeBindings = new List<InputBinding>();
                        compositeBindings.AddRange(_playerInput.actions.FindAction(actionName).bindings.Where(compositeBinding =>
                            compositeBinding.isPartOfComposite));
                        
                        // set all the icons
                        string replacement = "";
                        foreach (var compositeBinding in compositeBindings)
                        {
                            replacement +=
                                CompleteTextWithButtonPromptSprite.ReadAndReplaceBinding(match.Value, compositeBinding, spriteAsset);
                        }
                        message = message.Replace(match.Value, replacement);
                    }
                    else
                    {
                        // normal operation
                        string replacement =
                            CompleteTextWithButtonPromptSprite.ReadAndReplaceBinding(match.Value, binding, spriteAsset);
                        message = message.Replace(match.Value, replacement);
                    }
                }
            }

            _textBox.text = message;
        }

        private enum DeviceType
        {
            Gamepad = 0,
            Playstation = 1,
            Switch = 2,
            Keyboard = 3,
        }
    }
}