using System;
using System.Collections;
using Player.Movement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerDeathManager : MonoBehaviour//, IDataPersistence
    {
        [SerializeField] private float puddleDeathDelay = 2f;
        private float _deathPuddleTimer;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            PlayerMovement.onPlayerInPuddle += PuddleDeathTime;
            PlayerMovement.onPlayerLeftPuddle += ResetDeathTime;
        }

        private void OnDestroy()
        {
            PlayerMovement.onPlayerInPuddle -= PuddleDeathTime;
            PlayerMovement.onPlayerLeftPuddle -= ResetDeathTime;
        }

        #region Loading and Saving
        // public void LoadData(GameData data)
        // {
        //     _rb.position = data.position;
        //     // jumpCount = data.jumpCount;
        // }
        //
        // public void SaveData(GameData data)
        // {
        //     data.position = _rb.position;
        //     // data.jumpCount = jumpCount;
        // }
    
        #endregion


        #region puddle Handlig
        
        private void PuddleDeathTime()
        {
            // Debug.Log("started dying");

            StartCoroutine(nameof(DeathTimeCoroutine));
        }
        
        private IEnumerator DeathTimeCoroutine()
        {
            while (_deathPuddleTimer < puddleDeathDelay)
            {
                _deathPuddleTimer += Time.deltaTime;
                Debug.Log("Puddle Timer: " + _deathPuddleTimer);
                
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
        }
        #endregion

        private void ResetToCheckpoint()
        {
            Debug.Log("Death");
            // TODO: Create Game Over screen and hook it in here
            SceneManager.LoadSceneAsync("SampleScene");
        }
        
    }
}