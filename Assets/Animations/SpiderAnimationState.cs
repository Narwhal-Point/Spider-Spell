using System.Collections;
using System.Collections.Generic;
using Player.Movement;
using UnityEngine;

public class spiderAnimationState : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement player;

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
            animator.SetBool("isWalking",false);
            animator.SetBool("isFalling",false);
            animator.SetBool("isJumping",false);
        }
        if (player.movementState.ToString() == "Walking")
        {
            animator.SetBool("isWalking",true);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping",false);
            
            // Calculate the speed ratio
            float speedRatio = player.Rb.velocity.magnitude / player.walkSpeed;
            
            Debug.Log($"Speed Ratio: {speedRatio}");

            // Set the animation speed to the speed ratio
            animator.speed = speedRatio;
        }
        if (player.movementState.ToString() == "Falling" || player.movementState.ToString() == "Jumping")
        {
            animator.speed = 1f;
            animator.SetBool("isFalling", true);
            animator.SetBool("isWalking",false);
            animator.SetBool("isJumping",false);
        }
        
    }
}