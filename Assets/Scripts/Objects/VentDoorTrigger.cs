using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoorTrigger : MonoBehaviour
{
    [SerializeField] VentDoor ventDoor;

    private void OnTriggerStay(Collider other)
    {
        ventDoor.OpenDoor();
    }
}
