using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public Vector3 position;

    public Quaternion rotation;

    public int maxCollectables;

    public bool introCutscenePlayed;

    public bool witchCutscenePlayed;

    public bool mainRoomCutscenePlayed;

    // public int jumpCount;

    public bool journalCollected;

    // public Dictionary<string, GameObject> CollectedIngredients = new Dictionary<string, GameObject>();
    public SerializableDictionary<string, string> collectedIngredients = new SerializableDictionary<string, string>();
    public Vector3 spawnPoint = new Vector3(430.98f, 35.478f, 198f);

    public GameData()
    {
        this.position = spawnPoint;
        this.rotation = Quaternion.Euler(0,90,0);
        maxCollectables = 5;
        
        Debug.Log("Rotation GameData: " + rotation);
    }

    public int GetPercentageComplete()
    {
        int percentageCompleted = (collectedIngredients.Count * 100 / maxCollectables);
        return percentageCompleted;
    }
}