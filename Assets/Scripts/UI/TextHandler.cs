using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TextHandler : MonoBehaviour
    {
        public TextAsset textFile;
        public Image textBackground;
        public GameObject tutorialTextObject;
        [Tooltip("Use this if you don't want to use a text file")]
        public string backupText;
    
        private string[] _text;
        private string _displayText;
        private string _action;
        private bool _playerInArea;
        private int _currentLine;
        private TMP_Text _tutorialText;
        private SetTextToTextBox _textUI;

        private void Awake()
        {
            _tutorialText = tutorialTextObject.GetComponent<TMP_Text>();
            _textUI = tutorialTextObject.GetComponent<SetTextToTextBox>();
        }

        private void Start()
        {
            if (textFile)
            {
                _text = (textFile.text.Split('\n'));
            }
            else
            {
                _text = new string[1];
                _text[0] = backupText;
            }
        
            _tutorialText.enabled = false;
            textBackground.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentLine = 0;
            _displayText = _text[_currentLine];
            CheckForInput();

            if (!string.IsNullOrEmpty(_action))
            {
                _textUI.SetText(_displayText, _action);
            }
            else
                _textUI.SetText(_displayText);
        
            _tutorialText.enabled = true;
            textBackground.enabled = true;
            _playerInArea = true;
        }

        private void OnTriggerExit(Collider other)
        {
            DisableText();
        }

        private void Update()
        {
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
        
            if(!string.IsNullOrEmpty(_action))
                _textUI.SetText(_displayText, _action);
            else
                _textUI.SetText(_displayText);
        
        }

        private void CheckForInput()
        {
            if (Regex.IsMatch(_displayText, @"\[.*\]"))
            {
                Match match = Regex.Match(_displayText, @"\[(.*?)\]");
                if (match.Success)
                {
                    _action = match.Groups[1].Value;
                    Debug.Log(_action);
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
            _tutorialText.enabled = false;
            textBackground.enabled = false;
            _playerInArea = false;
            _currentLine = 0;
        }
    }
}
