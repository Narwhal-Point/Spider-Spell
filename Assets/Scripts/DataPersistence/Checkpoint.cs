using Audio;
using Interaction;
using UnityEngine;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private string text = "Press [Interact] to save";
        private ParticleSystem _starsVFX;
        private AudioManager _audioManager;

        private void Start()
        {
            _starsVFX = gameObject.GetComponent<ParticleSystem>();
            _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
        
        public void Interact()
        {
            DataPersistenceManager.instance.SaveGame();
            _starsVFX.Play();
            _audioManager.PlaySFX(_audioManager.checkpointSfx);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetInteractionText()
        {
            return text;
        }
    }
}
