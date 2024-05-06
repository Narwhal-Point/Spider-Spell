using System;
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
        [SerializeField] private float puddleDeathDelay = 2f;
        private float _deathPuddleTimer;
        private Rigidbody _rb;

        [SerializeField] private Volume volume;
        private Vignette _vignette;

        private void Start()
        {
            if (volume.profile.TryGet<Vignette>(out var v))
            {
                _vignette = v;
            }
            
            _rb = GetComponent<Rigidbody>();
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

            StartCoroutine(nameof(DeathTimeCoroutine));
        }
        
        private IEnumerator DeathTimeCoroutine()
        {
            while (_deathPuddleTimer < puddleDeathDelay)
            {
                // if the death time is slowed down or sped up this should be modified.
                _vignette.intensity.value += 0.002f;
                
                _deathPuddleTimer += Time.deltaTime;
                
                yield return null;
            }
            
            _deathPuddleTimer = 0;
            ResetToCheckpoint();
        }
        

        private void ResetDeathTime()
        {
            StopCoroutine(nameof(DeathTimeCoroutine));
            // Debug.Log("Puddle Timer: " + _deathPuddleTimer);
            _deathPuddleTimer = 0;
            _vignette.intensity.value = 0f;
        }
        #endregion

        private void ResetToCheckpoint()
        {
            // TODO: Create Game Over screen and hook it in here
            SceneManager.LoadSceneAsync("SampleScene");
        }
        
    }
}