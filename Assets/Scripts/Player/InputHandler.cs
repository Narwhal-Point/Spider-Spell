using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerMovement _player;
        // Start is called before the first frame update
        private void Awake()
        {
            _player.GetComponent<PlayerMovement>();
        }

        private void OnMove(InputValue value)
        {
            _player.OnMove(value);
        }

        private void OnSprint(InputValue value)
        {
            _player.OnSprint(value);
        }

        private void OnFire(InputValue value)
        {
            _player.OnFire(value);
        }

        private void OnCrouch(InputValue value)
        {
            _player.OnCrouch(value);
        }

        private void OnSlide(InputValue value)
        {
            _player.OnSlide(value);
        }

    }
}
