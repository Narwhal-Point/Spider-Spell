using System;
using UnityEngine;

namespace Interaction
{
    public class GrabQuestLog : MonoBehaviour, IInteractable
    {
        [SerializeField] private string text = "Press [Interact] to grab the Ingredient Page";
        [SerializeField] private GameObject questLog;

        private UI.QuestLog _logScript;

        private void Awake()
        {
            _logScript = questLog.GetComponent<UI.QuestLog>();
        }

        public void Interact()
        {
            questLog.SetActive(true);
            _logScript.OpenQuestLog();
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
    }
}