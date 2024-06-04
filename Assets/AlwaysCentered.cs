using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysCentered : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Get the screen's dimensions
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // Convert screen coordinates to world coordinates
        Vector3 centerWorldPos = Camera.main.ScreenToWorldPoint(screenSize / 2);

        // Set the object's position to the center of the screen
        transform.position = centerWorldPos;
    }
}
