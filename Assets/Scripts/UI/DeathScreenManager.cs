using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    // this is some of the worst code ever written in the history of humanity. Whoever reviews this, I'm sorry.
    public class DeathScreenManager : MonoBehaviour
    {
        [Tooltip("Time it takes for the death screen to play, the total respawn time is this value times two + 0.2")]
        [SerializeField] private float deathScreenTime;

        [Header("Images")]
        [Tooltip("The image with the sprite")]
        [SerializeField] private Image deathScreenImage;
        [Tooltip("Just a black Image")]
        [SerializeField] private Image blackImage;

        [Tooltip("all the black images, order should be top -> bottom -> right - > left")]
        [SerializeField] private Image[] images;
        [SerializeField] private PlayerInput playerInput;
        private Vector3[] _imageStartPos;
        private Vector3[] _imageEndPos;

        // the amount that needs to be subtracted / added to the images to make them move towards the correct position in the middle
        private const float LocalPosHeightEnd = 1188;
        private const float LocalPosWidthEnd = 2112;

        // delegate to let scripts now the player is allowed to respawn
        public delegate void DonePlayingDeathEffect();
        public static DonePlayingDeathEffect onDonePlayingDeathEffect;

        private void Start()
        {
            InitImagePositions();
            
            SubscribeToDeathEvent();
            
            EnableImages();
            
            // always play respawn effect on spawn.
            // Workaround for how our saving system works (reloads the scene)
            StartCoroutine(RespawnEffect());
        }

        private void OnDestroy()
        {
            UnSubscribeFromDeathEvent();
        }

        private void InitImagePositions()
        {
            _imageStartPos = new Vector3[images.Length];
            _imageEndPos = new Vector3[images.Length];
            
            for (int i = 0; i < images.Length; i++)
            {
                _imageStartPos[i] = images[i].transform.localPosition;
            }
            
            Vector3[] endPos = {
                new(images[0].transform.localPosition.x,
                    images[0].transform.localPosition.y - LocalPosHeightEnd, images[0].transform.localPosition.z), 
                new(images[1].transform.localPosition.x,
                    images[1].transform.localPosition.y + LocalPosHeightEnd, images[1].transform.localPosition.z),
                new(images[2].transform.localPosition.x - LocalPosWidthEnd,
                    images[2].transform.localPosition.y, images[2].transform.localPosition.z),
                new(images[3].transform.localPosition.x + LocalPosWidthEnd,
                    images[3].transform.localPosition.y, images[3].transform.localPosition.z)
            };

            _imageEndPos = endPos;
        }
        private void EnableImages()
        {
            // blackImage excluded because we don't want to enable that together with the others
            
            deathScreenImage.enabled = true;
            foreach (var image in images)
            {
                image.enabled = true;
            }
        }

        private void DisableImages()
        {
            deathScreenImage.enabled = false;
            blackImage.enabled = false;
            foreach (var image in images)
            {
                image.enabled = false;
            }
        }

        private void SubscribeToDeathEvent()
        {
            PlayerDeathManager.onPlayerDied += StartDeathScreen;
        }

        private void UnSubscribeFromDeathEvent()
        {
            PlayerDeathManager.onPlayerDied -= StartDeathScreen;
        }

        private void StartDeathScreen()
        {
            EnableImages();
            
            deathScreenImage.transform.localScale = new Vector3(1f, 1f, 1f);
            StartCoroutine(DeathEffect());
        }

        private IEnumerator DeathEffect()
        {
            float elapsedTime = 0f;

            while (elapsedTime < deathScreenTime)
            {
                float t = elapsedTime / deathScreenTime;

                // lerp the side screens towards the center
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].transform.localPosition = Vector3.Lerp(_imageStartPos[i], _imageEndPos[i], t * 0.85f);
                }
                
                // make the spider image smaller
                deathScreenImage.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(0.1f, 0.1f, 1f), t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the scale is set to the end scale when the loop is done
            deathScreenImage.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
            blackImage.enabled = true;
            
            // we can respawn!
            onDonePlayingDeathEffect?.Invoke();
        }
        
        
        private IEnumerator RespawnEffect()
        {
            EnableImages();
            blackImage.enabled = true;
            playerInput.enabled = false;
            
            // small delay to make the respawn effect look a bit better
            yield return new WaitForSeconds(0.2f);
            blackImage.enabled = false;
            
            for (int i = 0; i < images.Length; i++)
            {
                images[i].transform.localPosition = _imageEndPos[i];
            }
            
            Vector3[] startPos = new Vector3[images.Length];

            for (int i = 0; i < images.Length; i++)
            {
                startPos[i] = images[i].transform.localPosition;
            }
            
            
            Vector3[] endPos = {
                new(images[0].transform.localPosition.x,
                    images[0].transform.localPosition.y + LocalPosHeightEnd, images[0].transform.localPosition.z), 
                new(images[1].transform.localPosition.x,
                    images[1].transform.localPosition.y - LocalPosHeightEnd, images[1].transform.localPosition.z),
                new(images[2].transform.localPosition.x + LocalPosWidthEnd,
                    images[2].transform.localPosition.y, images[2].transform.localPosition.z),
                new(images[3].transform.localPosition.x - LocalPosWidthEnd,
                    images[3].transform.localPosition.y, images[3].transform.localPosition.z)
            };
            
            float elapsedTime = 0f;

            while (elapsedTime < deathScreenTime)
            {
                float t = elapsedTime / deathScreenTime;

                for (int i = 0; i < images.Length; i++)
                {
                    images[i].transform.localPosition = Vector3.Lerp(startPos[i], endPos[i], t * 2f);
                }

                deathScreenImage.transform.localScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 1f), new Vector3(2.5f, 2.5f, 2.5f), t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the scale is set to the end scale when the loop is done
            deathScreenImage.transform.localScale = new Vector3(1f, 1f, 1f);
            DisableImages();
            playerInput.enabled = true;

        }
    }
}
