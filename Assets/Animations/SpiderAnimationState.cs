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
        HandleWalkingSpeed();
        HandleSwitching();
    }

    private void HandleWalkingSpeed()
    {
        if (player.movementState == PlayerMovement.MovementState.Walking)
        {
            animator.speed = player.MoveSpeed / 10f;
            Debug.Log("animator speed: " + animator.speed);
            Debug.Log("MoveSpeed: " + player.MoveSpeed);
        }
        else
        {
            animator.speed = 1;
        }
        // if(player.MoveSpeed)
    }

    private void HandleSwitching()
    {
        switch (player.movementState.ToString())
        {
            case "Idle":
                animator.SetBool("isWalking", false);
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
                break;
            case "Walking":
                animator.SetBool("isWalking", true);
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
                break;
            case "Falling":
                animator.SetBool("isFalling", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isJumping", false);
                break;
        }

        if (player.jumpAnimation)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
            animator.SetBool("isWalking", false);
        }
    }
}