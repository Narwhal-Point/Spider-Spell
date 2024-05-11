using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    [CreateAssetMenu(fileName = "List of input sprite assests", menuName = "List of sprite Assets", order = 0)]
    public class ButtonPromptsSpriteAssests : ScriptableObject
    {
        public List<TMP_SpriteAsset> spriteAssets;
    }
}
