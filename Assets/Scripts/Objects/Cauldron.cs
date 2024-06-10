using Audio;
using UnityEngine;

namespace Objects
{
    public class Cauldron : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            AudioManager.LocationSpecificAudioSource.Add(_audioSource);
        }
        
        private void OnDestroy()
        {
            AudioManager.LocationSpecificAudioSource.Remove(_audioSource);
        }
    }
}