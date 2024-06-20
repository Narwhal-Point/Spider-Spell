using System;
using UnityEngine;

namespace Interaction
{
    public class GrabQuestLog : MonoBehaviour, IInteractable, IDataPersistence
    {
        [SerializeField] private string text = "Press [Interact] to grab the Ingredient Page";
        [SerializeField] private GameObject questLog;
        private bool _questLogCollected;
        private MeshRenderer _renderer;

        private UI.QuestLog _logScript;

        private void Start()
        {
            _renderer = gameObject.GetComponent<MeshRenderer>();
            Material newMaterial;
            if (!_questLogCollected)
            {
                newMaterial = Resources.Load<Material>("Material/open_book_ingredient");
            }
            else
            {
                newMaterial = Resources.Load<Material>("Material/open_book");
                Destroy(this);
            }

            _renderer.material = newMaterial;
            
            _logScript = questLog.GetComponent<UI.QuestLog>();
        }

        public void Interact()
        {
            questLog.SetActive(true);
            _logScript.OpenQuestLog();
           _renderer.material = Resources.Load<Material>("Material/open_book");
            
            Destroy(this);
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public string GetInteractionText()
        {
            return text;
        }

        public void LoadData(GameData data)
        {
            _questLogCollected = data.journalCollected;
        }

        public void SaveData(GameData data)
        {
        }
    }
}