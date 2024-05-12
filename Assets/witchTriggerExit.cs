using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class witchTriggerExit : MonoBehaviour
{
    [SerializeField] private GameObject witch;
    private GameObject tutorialTrigger;
    private void Start()
    {
        tutorialTrigger = GameObject.Find("Witch Tutorial Trigger");
        witch.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(tutorialTrigger);
        witch.SetActive(false);
    }
}
