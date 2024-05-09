using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class KeyboardMouseIconsExample : MonoBehaviour
    {
        public KeyboardIcons keyboardIcons;
        public MouseIcons mouseIcons;

        void Start()
        {
            // Get all input devices
            var devices = InputSystem.devices;
        
            // Iterate over each device
            foreach (var device in devices)
            {
                Debug.Log("Device Name: " + device.name);
                Debug.Log("Device Layout: " + device.layout);
        
                // Print out control paths
                foreach (var control in device.allControls)
                {
                    Debug.Log("Control Path: " + control.path);
                }
        
                Debug.Log("---------------------------------------");
            }
        }

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString,
            string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                icon = keyboardIcons.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = mouseIcons.GetSprite(controlPath);

            var textComponent = component.bindingText;
            // Grab Image component.
            var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");
            var imageComponent = imageGO.GetComponent<Image>();
            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

 [Serializable]
public struct KeyboardIcons
{
    // Alphabets
    public Sprite keyA;
    public Sprite keyB;
    public Sprite keyC;
    public Sprite keyD;
    public Sprite keyE;
    public Sprite keyF;
    public Sprite keyG;
    public Sprite keyH;
    public Sprite keyI;
    public Sprite keyJ;
    public Sprite keyK;
    public Sprite keyL;
    public Sprite keyM;
    public Sprite keyN;
    public Sprite keyO;
    public Sprite keyP;
    public Sprite keyQ;
    public Sprite keyR;
    public Sprite keyS;
    public Sprite keyT;
    public Sprite keyU;
    public Sprite keyV;
    public Sprite keyW;
    public Sprite keyX;
    public Sprite keyY;
    public Sprite keyZ;

    // Numerals
    public Sprite key0;
    public Sprite key1;
    public Sprite key2;
    public Sprite key3;
    public Sprite key4;
    public Sprite key5;
    public Sprite key6;
    public Sprite key7;
    public Sprite key8;
    public Sprite key9;

    // Function keys
    public Sprite keyF1;
    public Sprite keyF2;
    public Sprite keyF3;
    public Sprite keyF4;
    public Sprite keyF5;
    public Sprite keyF6;
    public Sprite keyF7;
    public Sprite keyF8;
    public Sprite keyF9;
    public Sprite keyF10;
    public Sprite keyF11;
    public Sprite keyF12;

    // Modifier keys
    public Sprite keyCtrl;
    public Sprite keyShift;
    public Sprite keyAlt;
    public Sprite keyWin;
    public Sprite keySpaceBar;

    // Special characters
    public Sprite keyDot;
    public Sprite keyComma;
    public Sprite keyQuestionMark;
    public Sprite keyApostrophe;
    public Sprite keyDoubleQuote;
    public Sprite keyBacktick; // Also known as Grave accent or Grave
    public Sprite keyUnderscore;

    // Navigation keys
    public Sprite keyArrowLeft;
    public Sprite keyArrowRight;
    public Sprite keyArrowUp;
    public Sprite keyArrowDown;

    public Sprite GetSprite(string controlPath)
{
    // Map control path to keyboard keys
    switch (controlPath)
    {
        // Alphabets
        case "a": return keyA;
        case "b": return keyB;
        case "c": return keyC;
        case "d": return keyD;
        case "e": return keyE;
        case "f": return keyF;
        case "g": return keyG;
        case "h": return keyH;
        case "i": return keyI;
        case "j": return keyJ;
        case "k": return keyK;
        case "l": return keyL;
        case "m": return keyM;
        case "n": return keyN;
        case "o": return keyO;
        case "p": return keyP;
        case "q": return keyQ;
        case "r": return keyR;
        case "s": return keyS;
        case "t": return keyT;
        case "u": return keyU;
        case "v": return keyV;
        case "w": return keyW;
        case "x": return keyX;
        case "y": return keyY;
        case "z": return keyZ;

        // Numerals
        case "1": return key1;
        case "2": return key2;
        case "3": return key3;
        case "4": return key4;
        case "5": return key5;
        case "6": return key6;
        case "7": return key7;
        case "8": return key8;
        case "9": return key9;
        case "0": return key0;

        // Function keys
        case "f1": return keyF1;
        case "f2": return keyF2;
        case "f3": return keyF3;
        case "f4": return keyF4;
        case "f5": return keyF5;
        case "f6": return keyF6;
        case "f7": return keyF7;
        case "f8": return keyF8;
        case "f9": return keyF9;
        case "f10": return keyF10;
        case "f11": return keyF11;
        case "f12": return keyF12;

        // Modifier keys
        case "ctrl": return keyCtrl;
        case "shift": return keyShift;
        case "alt": return keyAlt;
        case "win": return keyWin;

        // Special characters
        case "period": return keyDot;
        case "comma": return keyComma;
        case "questionmark": return keyQuestionMark;
        case "apostrophe": return keyApostrophe;
        case "doublequote": return keyDoubleQuote;
        case "backtick": return keyBacktick;
        case "underscore": return keyUnderscore;
        case "space": return keySpaceBar;

        // Navigation keys
        case "leftArrow": return keyArrowLeft;
        case "upArrow": return keyArrowUp;
        case "downArrow": return keyArrowDown;
        case "rightArrow": return keyArrowRight;

        default:
            return null;
    }
}

}


        [Serializable]
        public struct MouseIcons
        {
            // Define sprites for mouse buttons
            public Sprite leftButton;
            public Sprite rightButton;
            public Sprite middleButton;
            // Add more sprites for other buttons as needed

            public Sprite GetSprite(string controlPath)
            {
                // Map control path to mouse buttons
                switch (controlPath)
                {
                    case "leftButton": return leftButton;
                    case "rightButton": return rightButton;
                    case "middleButton": return middleButton;
                    // Add mappings for other buttons
                    default: return null;
                }
            }
        }
    }
}
