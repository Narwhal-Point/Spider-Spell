using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public static class CompleteTextWithButtonPromptSprite
    {
        public static string ReadAndReplaceBinding(string textToDisplay, InputBinding binding,
            TMP_SpriteAsset spriteAsset)
        {
            string stringButtonName = binding.ToString();
            stringButtonName = RenameInput(stringButtonName);

            // find the action between '[' and ']' and replace this with the corresponding icon.
            textToDisplay = Regex.Replace(textToDisplay, @"\[.*\]",
                $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">");
            return textToDisplay;
        }

        private static string RenameInput(string stringButtonName)
        {
            // regex for removing the action map from the string
            stringButtonName = Regex.Replace(stringButtonName, @"[^""]*:", String.Empty);
            // regex for removing the current device from the string
            stringButtonName = Regex.Replace(stringButtonName, @"\[.*\]", String.Empty);

            // if we want to support more devices, add them here.
            stringButtonName = stringButtonName.Replace("<Gamepad>/", "Xbox_");
            stringButtonName = stringButtonName.Replace("<Keyboard>/", "Keyboard_");
            stringButtonName = stringButtonName.Replace("<Mouse>/", "Mouse_");
            stringButtonName = stringButtonName.Replace("<DualShockGamepad>/", "playstation_");
            stringButtonName = stringButtonName.Replace("<SwitchProControllerHID>/", "Switch_");
            
            return stringButtonName;
        }
    }
}