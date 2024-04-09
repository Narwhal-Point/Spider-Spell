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
        if (player.movementState.ToString() == "Walking")
        {
            animator.SetBool("isWalking",true);
        }
        else
        {
            animator.SetBool("isWalking",false);
        }
    }
}