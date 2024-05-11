using UnityEngine;

namespace UI
{
    public class TextBoxHandler : MonoBehaviour
    {
        [Header("Add your text here")]
        [Tooltip("The file containing all your text for the current area trigger")]
        public TextAsset textFile;
        
        [Tooltip("Use this if you don't want to use a text file")]
        public string backupText;

        [Header("Textbox")]
        [Tooltip("The background of your text area, should contain your text components")]
        public GameObject textBackground;

        [Tooltip("The object displaying your text, should be a child of textBackground")]
        public GameObject textObject;

        // class that handles setting the text
        private SetTextToTextBox _textUI;
        

        [Header("interact key")]
        // Interact button in the right corner.
        [Tooltip("Interact button in the right corner of the text box")]
        public GameObject displayInteractKeyHandler;
        private SetTextToTextBox _interactTextUI;
        
        [Tooltip("Toggle this to toggle the interact key in the bottom right corner of the text box")]
        [SerializeField] private bool enableInteractKey = true;

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
            if (!textBackground)
            {
                textBackground = GameObject.Find("Text Box Background Base");
            }

            if (!textObject)
            {
                textObject = GameObject.Find("UI Text Base");
            }

            if (!displayInteractKeyHandler)
            {
                displayInteractKeyHandler = GameObject.Find("Text Interact Key Handler Base");
            }

            _textUI = textObject.GetComponent<SetTextToTextBox>();
        }

        private void Start()
        {
            // set the button for the corner
                _interactTextUI = displayInteractKeyHandler.GetComponent<SetTextToTextBox>();
                _interactTextUI.SetText("[Interact]");
            
            if(!enableInteractKey)
            {
                displayInteractKeyHandler.SetActive(false);
            }

            if (textFile)
            {
                _text = (textFile.text.Split('\n'));
            }
            else
            {
                _text = new string[1];
                _text[0] = backupText;
            }

            textObject.SetActive(false);
            textBackground.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentLine = 0;
            _displayText = _text[_currentLine];

            // set the text
            _textUI.SetText(_displayText);


            // enable the text boxes
            if(enableInteractKey)
                displayInteractKeyHandler.SetActive(true);
            textObject.SetActive(true);
            textBackground.SetActive(true);
            _playerInArea = true;
        }

        private void OnTriggerExit(Collider other)
        {
            DisableText();
        }

        private void Update()
        {
            // no player in the trigger range
            if (!_playerInArea)
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
            }

            // keep updating text ui for if input device changes
            if(enableInteractKey)
                _interactTextUI.SetText("[Interact]");
            _textUI.SetText(_displayText);
        }

        private void DisableText()
        {
            displayInteractKeyHandler.SetActive(false);
            if(enableInteractKey)
                textObject.SetActive(false);
            textBackground.SetActive(false);
            _playerInArea = false;
            _currentLine = 0;
        }
    }
}