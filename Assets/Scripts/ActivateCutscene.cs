using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playableDirector.Play();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
