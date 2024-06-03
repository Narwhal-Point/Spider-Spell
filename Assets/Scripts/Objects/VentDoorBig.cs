using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoorBig : MonoBehaviour
{
    [SerializeField] private float openingSpeed = 250f;
    [SerializeField] private float maxOpeningAngle = 90;
    private bool isOpening = false;
    private bool isClosing = false;
    private bool isWaiting = false;
    Vector3 startRotation;
    private float x;
    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
        Debug.Log(startRotation);
        x = startRotation.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            x += openingSpeed * Time.deltaTime;
            if (x > startRotation.x + maxOpeningAngle)
            {
                isOpening = false;
                Invoke("CloseDoor", 5f);
                isWaiting = true;
            }
            transform.localRotation = Quaternion.Euler(x, 90, 90);
        }
        else if (isClosing)
        {
            x -= openingSpeed * Time.deltaTime;
            if (x < startRotation.x)
            {
                x = startRotation.x;
                isClosing = false;
            }
            transform.localRotation = Quaternion.Euler(x, 90, 90);
        }
    }

    private void CloseDoor()
    {
        isClosing = true;
        isWaiting = false;
    }

    public void OpenDoor()
    {
        if (!isClosing && !isWaiting && !isOpening) 
        {
            isOpening = true;
        }
    }
}