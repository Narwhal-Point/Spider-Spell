using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public static class CompleteTextWithButtonPromptSprite
    {
        public static string ReadAndReplaceBinding(string textToDisplay, InputBinding actionNeeded, TMP_SpriteAsset spriteAsset)
        {
            string stringButtonName = actionNeeded.ToString();
            stringButtonName = RenameInput(stringButtonName);

            textToDisplay = textToDisplay.Replace("BUTTONPROMPT",
                $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">");
            return textToDisplay;
        }

        private static string RenameInput(string stringButtonName)
        {
            // regex for removing the action map from the string
            stringButtonName = Regex.Replace(stringButtonName, @"[^""]*:", String.Empty);
            // regex for removing the current device from the string
            stringButtonName = Regex.Replace(stringButtonName, @"\[.*\]", String.Empty);

            stringButtonName = stringButtonName.Replace("<Gamepad>/", "Gamepad_");
            stringButtonName = stringButtonName.Replace("<Keyboard>/", "Keyboard_");
            stringButtonName = stringButtonName.Replace("<Mouse>/", "Mouse_");

            return stringButtonName;
        }
    }
}
