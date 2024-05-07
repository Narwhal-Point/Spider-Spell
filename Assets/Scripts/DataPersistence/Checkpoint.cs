using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour//, IDataPersistence
{
    private int jumpCount;
    private Rigidbody _rb;
    
    #region Loading and Saving
    // public void LoadData(GameData data)
    // {
    //     _rb.position = data.position;
    //     jumpCount = data.jumpCount;
    // }
    //
    // public void SaveData(GameData data)
    // {
    //     data.position = _rb.position;
    //     data.jumpCount = jumpCount;
    // }
    
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        _rb = player.GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void OnTriggerEnter()
    {
        Debug.Log("test");
        DataPersistenceManager.instance.SaveGame();
    }
}
