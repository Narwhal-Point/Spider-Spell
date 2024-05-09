using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PistonManager : MonoBehaviour
{
    [SerializeField] GameObject[] pistons;
    [SerializeField] Vector3 targetDirection;
    private Vector3 moveDirection;
    [SerializeField] float maxMoveWidth = 7f;
    private float currentMoved = 0f;
    [SerializeField] float moveSpeed = 4f;
    private bool moveFirstGroup = true;
    private bool movingOut = true;
    private bool waiting = true;
    [SerializeField] float waitDuration = 0.5f;
    private float waitTimer = 0f;
    [SerializeField] AudioSource shoveSFX;

    // Start is called before the first frame update
    void Start()
    {
        targetDirection = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFirstGroup)
        {
            if (pistons.Length > 0)
            {
                MovePistions(0);
            }
        }
        else
        {
            if (pistons.Length > 1)
            {
                MovePistions(1);
            }
        }
    }

    public void MovePistions(int startIndex)
    {
        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                waiting = false;
                waitTimer = 0f;
                shoveSFX.Play();
            }
        }
        else
        {
            if (movingOut)
            {

                for (int i = startIndex; i < pistons.Length; i += 2)
                {
                    moveDirection = targetDirection.normalized * moveSpeed * Time.deltaTime;
                    pistons[i].transform.position += moveDirection;
                }
                currentMoved += moveDirection.magnitude;
                if (currentMoved >= maxMoveWidth)
                {
                    movingOut = false;
                    waiting = true;
                    shoveSFX.Stop();
                }
            }
            if (!movingOut)
            {
                for (int i = startIndex; i < pistons.Length; i += 2)
                {
                    moveDirection = -targetDirection.normalized * moveSpeed * Time.deltaTime;
                    pistons[i].transform.position += moveDirection;
                }
                currentMoved -= moveDirection.magnitude;
                if (currentMoved <= 0)
                {
                    movingOut = true;
                    moveFirstGroup = (moveFirstGroup) ? false : true;
                    waiting = true;
                    shoveSFX.Stop();
                }
            }
        }
    }
}
