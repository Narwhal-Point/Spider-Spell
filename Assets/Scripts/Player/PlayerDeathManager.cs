using System.Collections;
using Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerDeathManager : MonoBehaviour
    {
        public delegate void PlayerDied();
        public static PlayerDied onPlayerDied;
        
        [Header("Puddles")] 
        [Tooltip("Amount of time it takes before the player dies")]
        [SerializeField] private float puddleDeathDelay = 2f;

        [Tooltip("Speed the vignette effect dissapears after leaving the puddle")] [SerializeField]
        private float vignetteDissapearSpeed = 1f;

        private float _deathPuddleTimer;
        [SerializeField] private Volume volume;
        private Vignette _vignette;
        private AudioManager audioManager;
        private void Start()
        {
            InitVignette();
            SubscribeToEvents();
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void SubscribeToEvents()
        {
            Movement.PlayerMovement.onPlayerInPuddle += PuddleDeathTime;
            Movement.PlayerMovement.onPlayerLeftPuddle += ResetDeathTime;
            UI.DeathScreenManager.onDonePlayingDeathEffect += ResetToCheckpoint;
        }
        
        private void UnsubscribeFromEvents()
        {
            Movement.PlayerMovement.onPlayerInPuddle -= PuddleDeathTime;
            Movement.PlayerMovement.onPlayerLeftPuddle -= ResetDeathTime;
            UI.DeathScreenManager.onDonePlayingDeathEffect -= ResetToCheckpoint;
        }


        #region puddle Handling

        private void InitVignette()
        {
            if (volume.profile.TryGet<Vignette>(out var v))
            {
                _vignette = v;
            }
        }
        
        private void PuddleDeathTime()
        {
            StopCoroutine(nameof(DisableVignette));
            StartCoroutine(nameof(DeathTimeCoroutine));
        }

        private IEnumerator DeathTimeCoroutine()
        {
            // start dying sound
            audioManager.PlayLoopSFX(audioManager.dyingSound);
            audioManager.PlayLoopSFX(audioManager.acidSound);
            
            while (_deathPuddleTimer < puddleDeathDelay)
            {
                // make the vignette always reach 1 in '_deathPuddleTimer' time.
                // Calculate the rate of change per frame
                float rateOfChange = 1 / puddleDeathDelay;
                // Multiply by Time.deltaTime to get the change for this frame
                _vignette.intensity.value += rateOfChange * Time.deltaTime;


                _deathPuddleTimer += Time.deltaTime;

                yield return null;
            }
            
            // stop dying sound
            audioManager.StopSFX(audioManager.dyingSound);
            audioManager.StopSFX(audioManager.acidSound);

            _deathPuddleTimer = 0;
            KillPlayer();
        }

        private IEnumerator DisableVignette()
        {
            while (_vignette.intensity.value > 0)
            {
                _vignette.intensity.value -= vignetteDissapearSpeed * Time.deltaTime;

                yield return null;
            }
        }


        private void ResetDeathTime()
        {
            StopCoroutine(nameof(DeathTimeCoroutine));
            
            // stop dying sound
            audioManager.StopSFX(audioManager.dyingSound);
            audioManager.StopSFX(audioManager.acidSound);
            
            _deathPuddleTimer = 0;
            StartCoroutine(DisableVignette());
        }

        #endregion

        public void KillPlayer()
        {
            InputManager.instance.DisableAllInputsButMenu();
            audioManager.PlaySFX(audioManager.gameOver);
            onPlayerDied?.Invoke();
        }
        private void ResetToCheckpoint()
        {
            SceneManager.LoadSceneAsync("SampleScene");
        }
    }
}