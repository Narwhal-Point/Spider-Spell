using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    public long lastUpdated;
    
    public Vector3 position;

    public int jumpCount;

    public Vector3 spawnPoint = new Vector3(13,-445,-192);

    public GameData()
    {
        this.position = spawnPoint;
        this.jumpCount = 0;
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
