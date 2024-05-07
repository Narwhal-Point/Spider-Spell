using UnityEngine;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour//, IDataPersistence
    {
        void OnTriggerEnter(Collider collider)
        {
            Debug.Log("test");
            DataPersistenceManager.instance.SaveGame();
        }
    }
}
