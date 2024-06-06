using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoorTriggerBig : MonoBehaviour
{
    [SerializeField] VentDoorBig ventDoor;

    private void OnTriggerStay(Collider other)
    {
        ventDoor.OpenDoor();
    }
}
