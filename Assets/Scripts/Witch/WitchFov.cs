using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchFov : MonoBehaviour
{
    [Tooltip("Range from which the witch will start chasing the player")]
    public float radius;
    
    [Tooltip("FOV Angle")]
    [Range(0,360)]
    public float angle;

    [NonSerialized] public GameObject PlayerRef;

    [Tooltip("Select the layers the witch should try to chase and attack")]
    public LayerMask targetMask;
    [Tooltip("Select the layers the witch should not be able to see through")]
    public LayerMask obstructionMask;

    [NonSerialized] public bool CanSeePlayer;
    

    private void Start()
    {
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    CanSeePlayer = true;
                }
                else
                {
                    CanSeePlayer = false;
                }
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else if (CanSeePlayer)
        {
            CanSeePlayer = false;
        }
    }
}
