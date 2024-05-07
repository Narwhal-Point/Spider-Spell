using System.Collections;
using Player.Movement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerDeathManager : MonoBehaviour
    {
        [Header("Puddles")]
        [SerializeField] private float puddleDeathDelay = 2f;
        [Tooltip("Speed the vignette effect dissapears after leaving the puddle")]
        [SerializeField] private float vignetteDissapearSpeed = 1f;
        private float _deathPuddleTimer;

        [SerializeField] private Volume volume;
        private Vignette _vignette;

        private void Start()
        {
            if (volume.profile.TryGet<Vignette>(out var v))
            {
                _vignette = v;
            }
            
            PlayerMovement.onPlayerInPuddle += PuddleDeathTime;
            PlayerMovement.onPlayerLeftPuddle += ResetDeathTime;
        }

        private void OnDestroy()
        {
            PlayerMovement.onPlayerInPuddle -= PuddleDeathTime;
            PlayerMovement.onPlayerLeftPuddle -= ResetDeathTime;
        }


        #region puddle Handlig
        
        private void PuddleDeathTime()
        {
            StopCoroutine(nameof(DisableVignette));
            StartCoroutine(nameof(DeathTimeCoroutine));
        }
        
        private IEnumerator DeathTimeCoroutine()
        {
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
            
            _deathPuddleTimer = 0;
            ResetToCheckpoint();
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
            _deathPuddleTimer = 0;
            StartCoroutine(DisableVignette());
        }
        #endregion

        private void ResetToCheckpoint()
        {
            // TODO: Create Game Over screen and hook it in here
            SceneManager.LoadSceneAsync("SampleScene");
        }
        
    }
}