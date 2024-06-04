using System.Collections;
using UnityEngine;

namespace Player
{
    public class AnimateEyes : MonoBehaviour
    {
        [SerializeField] private Material eyeMaterial;

        [SerializeField] private float minBlinkStartDelay = 10f;
        [SerializeField] private float maxBlinkStartDelay = 30f;

        [SerializeField] private float blinkDelay = 0.03f;

        // values for the texture to have the correct position
        private readonly float[] _offsets = { 0f, 0.17f, 0.34f, 0.5f };
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        void Start()
        {
            StartCoroutine(BlinkAnimation());
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        // Recursive function to make it so the spider can blink more than once.
        private IEnumerator BlinkAnimation()
        {
            // Wait for a random time before starting the blink animation
            yield return new WaitForSeconds(Random.Range(minBlinkStartDelay, maxBlinkStartDelay));

            for (int i = 0; i < _offsets.Length; i++)
            {
                eyeMaterial.SetTextureOffset(BaseMap, new Vector2(_offsets[i], 0));
                yield return new WaitForSeconds(blinkDelay);
            }

            for (int i = _offsets.Length - 1; i >= 0; i--)
            {
                eyeMaterial.SetTextureOffset(BaseMap, new Vector2(_offsets[i], 0));
                yield return new WaitForSeconds(blinkDelay);
            }

            StartCoroutine(BlinkAnimation());
        }
    }
}