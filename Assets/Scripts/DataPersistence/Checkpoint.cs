using UnityEngine;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour//, IDataPersistence
    {
        ParticleSystem starsVFX;
        AudioSource magicSFX;

        private void Start()
        {
            starsVFX = gameObject.GetComponent<ParticleSystem>();
            magicSFX = gameObject.GetComponent<AudioSource>();
        }

        void OnTriggerEnter(Collider collider)
        {
            Debug.Log("test");
            DataPersistenceManager.instance.SaveGame();
            starsVFX.Play();
            magicSFX.Play();
        }
    }
}
