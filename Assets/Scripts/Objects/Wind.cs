using Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{

    [SerializeField] Vector3 windDirection;
    [SerializeField] float windStrength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("blowing");
            PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
            Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                otherRb.drag = 0f;
                movement.enabled = false;
                otherRb.AddForce(windDirection * windStrength, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement movement = other.gameObject.GetComponent<PlayerMovement>();
        Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>();
        if (otherRb != null)
        {
            movement.enabled = true;
            otherRb.AddForce(windDirection * windStrength, ForceMode.Impulse);
        }
    }

}
