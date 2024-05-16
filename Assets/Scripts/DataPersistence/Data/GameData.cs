using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public long lastUpdated;
    
    public Vector3 position;

    public Quaternion rotation;

    public int jumpCount;
    
    public Vector3 spawnPoint = new Vector3(430.98f, 35.478f, 118.32f);

    public GameData()
    {
        this.position = spawnPoint;
        this.jumpCount = 0;
        this.rotation = Quaternion.Euler(0,90,0);
        Debug.Log("Rotation GameData: " + rotation);
    }

    public int GetPercentageComplete()
    {
        int percentageCompleted = (jumpCount * 100 / 10);
        if (percentageCompleted > 100)
        {
            percentageCompleted = 100;
            return percentageCompleted;
        }
        return percentageCompleted;
    }
}
