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
        
        // void Start()
        // {
        //     // Get all input devices
        //     var devices = InputSystem.devices;
        //
        //     // Iterate over each device
        //     foreach (var device in devices)
        //     {
        //         Debug.Log("Device Name: " + device.name);
        //         Debug.Log("Device Layout: " + device.layout);
        //
        //         // Print out control paths
        //         foreach (var control in device.allControls)
        //         {
        //             Debug.Log("Control Path: " + control.path);
        //         }
        //
        //         Debug.Log("---------------------------------------");
        //     }
        // }

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

    // Navigation keys
    public Sprite keyArrowLeft;
    public Sprite keyArrowRight;
    public Sprite keyArrowUp;
    public Sprite keyArrowDown;
    
    // Additional keys
    public Sprite keyTab;
    public Sprite keyTilde;
    public Sprite keyAsterisk;
    public Sprite keyBackspace;
    public Sprite keyBracketLeft;
    public Sprite keyBracketRight;
    public Sprite keyCapsLock;
    public Sprite keyCtrl;
    public Sprite keyDelete;
    public Sprite keyEnd;
    public Sprite keyEnter;
    public Sprite keyEscape;
    public Sprite keyHome;
    public Sprite keyInsert;
    public Sprite keyLessThan;
    public Sprite keyGreaterThan;
    public Sprite keyMinus;
    public Sprite keyNumLock;
    public Sprite keyPageDown;
    public Sprite keyPageUp;
    public Sprite keyPlus;
    public Sprite keyQuestionMark;
    public Sprite keyQuote;
    public Sprite keySemicolon;
    public Sprite keyShift;
    public Sprite keyBackslash;
    public Sprite keySpacebar;

    // Numpad keys
    public Sprite keyNumpad0;
    public Sprite keyNumpad1;
    public Sprite keyNumpad2;
    public Sprite keyNumpad3;
    public Sprite keyNumpad4;
    public Sprite keyNumpad5;
    public Sprite keyNumpad6;
    public Sprite keyNumpad7;
    public Sprite keyNumpad8;
    public Sprite keyNumpad9;
    

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
        
        // Navigation keys
        case "leftArrow": return keyArrowLeft;
        case "upArrow": return keyArrowUp;
        case "downArrow": return keyArrowDown;
        case "rightArrow": return keyArrowRight;
        
        // Additional keys
        case "tab": return keyTab;
        case "tilde": return keyTilde;
        case "asterisk": return keyAsterisk;
        case "backspace": return keyBackspace;
        case "leftBracket": return keyBracketLeft;
        case "rightBracket": return keyBracketRight;
        case "capsLock": return keyCapsLock;
        case "ctrl": return keyCtrl;
        case "leftCtrl": return keyCtrl;
        case "rightCtrl": return keyCtrl;
        case "delete": return keyDelete;
        case "end": return keyEnd;
        case "enter": return keyEnter;
        case "escape": return keyEscape;
        case "home": return keyHome;
        case "insert": return keyInsert;
        case "lessThan": return keyLessThan;
        case "greaterThan": return keyGreaterThan;
        case "minus": return keyMinus;
        case "numLock": return keyNumLock;
        case "pageDown": return keyPageDown;
        case "pageUp": return keyPageUp;
        case "plus": return keyPlus;
        case "questionMark": return keyQuestionMark;
        case "quote": return keyQuote;
        case "semicolon": return keySemicolon;
        case "shift": return keyShift;
        case "leftShift": return keyShift;
        case "rightShift": return keyShift;
        case "backslash": return keyBackslash;
        case "space": return keySpacebar;

        // Numpad keys
        case "numpad0": return keyNumpad0;
        case "numpad1": return keyNumpad1;
        case "numpad2": return keyNumpad2;
        case "numpad3": return keyNumpad3;
        case "numpad4": return keyNumpad4;
        case "numpad5": return keyNumpad5;
        case "numpad6": return keyNumpad6;
        case "numpad7": return keyNumpad7;
        case "numpad8": return keyNumpad8;
        case "numpad9": return keyNumpad9;

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
