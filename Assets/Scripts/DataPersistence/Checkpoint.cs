using Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private string text = "Press [Interact] to save";
        ParticleSystem starsVFX;
        AudioManager audioManager;

        private void Start()
        {
            starsVFX = gameObject.GetComponent<ParticleSystem>();
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
        
        public void Interact()
        {
            DataPersistenceManager.instance.SaveGame();
            starsVFX.Play();
            audioManager.PlaySFX(audioManager.checkpointSFX);
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
