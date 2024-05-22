using UnityEngine;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour//, IDataPersistence
    {
        ParticleSystem starsVFX;
        [SerializeField] AudioManager audioManager;

        private void Start()
        {
            starsVFX = gameObject.GetComponent<ParticleSystem>();
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
