using Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataPersistence
{
    public class Checkpoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private string text = "Press [Interact] to save";
        public void Interact()
        {
            DataPersistenceManager.instance.SaveGame();
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
