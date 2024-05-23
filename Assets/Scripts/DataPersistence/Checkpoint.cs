using UnityEngine;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour//, IDataPersistence
    {
        ParticleSystem starsVFX;
        AudioManager audioManager;

        private void Start()
        {
            starsVFX = gameObject.GetComponent<ParticleSystem>();
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }

        void OnTriggerEnter(Collider collider)
        {
            Debug.Log("test");
            DataPersistenceManager.instance.SaveGame();
            starsVFX.Play();
            audioManager.PlaySFX(audioManager.checkpointSFX);
        }
    }
}
