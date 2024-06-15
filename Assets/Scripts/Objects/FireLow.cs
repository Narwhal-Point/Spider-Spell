using Player;
using UnityEngine;

namespace Objects
{
    public class FireLow : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Debug.Log("blowing");
                PlayerDeathManager deathManager = other.gameObject.GetComponent<PlayerDeathManager>();
                deathManager.KillPlayer();
            }
        }
    }
}