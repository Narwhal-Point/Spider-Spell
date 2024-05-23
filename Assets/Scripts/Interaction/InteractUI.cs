using System;
using TMPro;
using UI;
using UnityEngine;

namespace Interaction
{
    public class InteractUI : MonoBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private PlayerInteract playerInteract;
        [SerializeField] private string interactText;

        private SetTextToTextBox _textUI;

        private void Start()
        {
            _textUI = ui.GetComponent<SetTextToTextBox>();
        }

        private void Update()
        {
            if (playerInteract.GetInteractableObject() != null)
                ShowUI(playerInteract.GetInteractableObject());
            else
                HideUI();
        }

        public void ShowUI(IInteractable interactable)
        {
            _textUI.SetText(interactable.GetInteractionText());
            ui.SetActive(true);
        }

        public void HideUI()
        {
            ui.SetActive(false);
        }
    }
}