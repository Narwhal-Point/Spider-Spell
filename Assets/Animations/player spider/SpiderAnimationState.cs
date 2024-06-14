using Player.Movement;
using UnityEngine;

namespace Animations
{
    public class SpiderAnimationState : MonoBehaviour
    {
        public Animator animator;
        public PlayerMovement player;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsFalling = Animator.StringToHash("isFalling");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (player.movementState.ToString() == "Idle")
            {
                animator.speed = 1f;
                animator.SetBool(IsWalking,false);
                animator.SetBool(IsFalling,false);
                animator.SetBool(IsJumping,false);
            }
            if (player.movementState.ToString() == "Walking")
            {
                animator.SetBool(IsWalking,true);
                animator.SetBool(IsFalling, false);
                animator.SetBool(IsJumping,false);
                
                // Calculate the speed ratio
                float speedRatio = player.Rb.velocity.magnitude / player.walkSpeed;
                
                // Set the animation speed to the speed ratio
                animator.speed = speedRatio;
            }
            if (player.movementState.ToString() == "Falling" || player.movementState.ToString() == "Jumping" || player.movementState.ToString() == "Swinging")
            {
                animator.speed = 1f;
                animator.SetBool(IsFalling, true);
                animator.SetBool(IsWalking,false);
                animator.SetBool(IsJumping,false);
            }
        
        }
    }
}