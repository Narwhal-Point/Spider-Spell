using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace MainMenu
{
    public class Menu : MonoBehaviour
    {
        [Header("First Selected Button")] [SerializeField]
        private Button firstSelected;

        private InputSystemUIInputModule _iptmod;
        private InputAction _navigate;
        private InputAction _point;
        private bool _usingNavigateAction;

        protected virtual void OnEnable()
        {
            _iptmod = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
            _navigate = _iptmod.move.action;
            _point = _iptmod.point.action;
        }

        protected virtual void Update()
        {
            if (_navigate.WasPerformedThisFrame())
                _usingNavigateAction = true;
            else if (_point.WasPerformedThisFrame())
                _usingNavigateAction = false;
            // Periodically check for gamepad presence until the first selected button is set
            HandleCanvasActivation(firstSelected);
        }

        private void HandleCanvasActivation(Button firstSelectedButton)
        {
            if (!EventSystem.current.currentSelectedGameObject && _usingNavigateAction)
            {
                SetSelectedGameObjectIfGamepad(firstSelectedButton.gameObject);
            }
            else if (!_usingNavigateAction)
            {
                SetSelectedGameObjectIfGamepad(null);
            }
        }

        private void SetSelectedGameObjectIfGamepad(GameObject gameObjectToSelect)
        {
            if (_usingNavigateAction)
            {
                EventSystem.current.SetSelectedGameObject(gameObjectToSelect);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}