using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class TextHandlerTrigger : MonoBehaviour
    {
        [Tooltip("The file containing all your text for the current area trigger")]
        public TextAsset textFile;
        
        [Tooltip("The background of your text area, should contain your text components")]
        public Image textBackground;
        
        [Tooltip("The object displaying your text, should be a child of textBackground")]
        public GameObject textDisplayObject;
        // class that handles setting the text
        private SetTextToTextBox _textUI; 
        
        [Tooltip("Use this if you don't want to use a text file")]
        public string backupText;

        // interact button in the right corner
        private GameObject _displayInteractKeyHandler;
        private SetTextToTextBox _interactTextUI;
        
        // the array that will contain all the text
        private string[] _text;
        // array index
        private int _currentLine;
        // the line that is currently displayed
        private string _displayText;
        // what button prompt needs to be shown. [Jump] shows the button prompts for jump,
        // depending on the active input device
        private string _action;
        // check if the player is inside the trigger
        private bool _playerInArea;

        private void Awake()
        {
            _displayInteractKeyHandler = GameObject.Find("Text Interact Key Handler");
            _textUI = textDisplayObject.GetComponent<SetTextToTextBox>();
        }

        private void Start()
        {
            // set the button for the corner
            _interactTextUI = _displayInteractKeyHandler.GetComponent<SetTextToTextBox>();
            _interactTextUI.SetText("BUTTONPROMPT", "Interact");
            if (textFile)
            {
                _text = (textFile.text.Split('\n'));
            }
            else
            {
                _text = new string[1];
                _text[0] = backupText;
            }
        
            textDisplayObject.SetActive(false);
            textBackground.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentLine = 0;
            _displayText = _text[_currentLine];
            CheckForInput();

            // set the text
            if (!string.IsNullOrEmpty(_action))
            {
                _textUI.SetText(_displayText, _action);
            }
            else
                _textUI.SetText(_displayText);
        
            // enable the text boxes
            _displayInteractKeyHandler.SetActive(true);
            textDisplayObject.SetActive(true);
            textBackground.enabled = true;
            _playerInArea = true;
        }

        private void OnTriggerExit(Collider other)
        {
            DisableText();
        }

        private void Update()
        {
            // no player in the trigger range
            if(!_playerInArea)
                return;
        
            
            if (InputManager.instance.InteractInput)
            {
                _currentLine++;
            
                if (_currentLine >= _text.Length)
                {
                    DisableText();
                    return;
                }
            
                _displayText = _text[_currentLine];
            
                CheckForInput();
            }
        
            // keep updating text ui for if input device changes
            _interactTextUI.SetText("BUTTONPROMPT", "Interact");
            
            if(!string.IsNullOrEmpty(_action))
                _textUI.SetText(_displayText, _action);
            else
                _textUI.SetText(_displayText);
        
        }

        private void CheckForInput()
        {
            // checks for a input action between '[' and ']'
            if (Regex.IsMatch(_displayText, @"\[.*\]"))
            {
                Match match = Regex.Match(_displayText, @"\[(.*?)\]");
                if (match.Success)
                {
                    _action = match.Groups[1].Value;
                }
                _displayText = Regex.Replace(_displayText, @"\[.*\]", String.Empty);
            }
            else
            {
                _action = "";
            }
        }

        private void DisableText()
        {
            _displayInteractKeyHandler.SetActive(false);
            textDisplayObject.SetActive(false);
            textBackground.enabled = false;
            _playerInArea = false;
            _currentLine = 0;
        }
    }
}
