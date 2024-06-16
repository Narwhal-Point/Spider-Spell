using Audio;
using Collectables;
using UI;
using UnityEngine;

namespace Objects
{
    public class Cauldron : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private VictoryScreenManager victoryScreenManager;
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            AudioManager.LocationSpecificAudioSource.Add(_audioSource);
        }
        
        private void OnDestroy()
        {
            AudioManager.LocationSpecificAudioSource.Remove(_audioSource);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (CollectableManager.instance.CollectedAll())
            {
                if(other.collider.CompareTag("Player"))
                    victoryScreenManager.OpenVictoryScreen();
            }
        }
    }
}