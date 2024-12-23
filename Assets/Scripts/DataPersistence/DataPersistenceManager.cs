using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
   [Header("Debugging")] 
   [SerializeField] private bool disableDataPersistence = false;
   [SerializeField] private bool initializeDataIfNull = false;
   [SerializeField] private bool overrideSelectedProfileId = false;
   [SerializeField] private string testSelectedProfileId = "test";
   
   [Header("File Storage Config")] 
   
   [SerializeField] private string fileName;

   private GameData gameData;

   private List<IDataPersistence> dataPersistenceObjects;
   
   private FileDataHandler dataHandler;

   private string selectedProfileId = "";
   public static DataPersistenceManager instance { get; private set; }

   private void Awake()
   {
      if (instance != null)
      {
         Debug.Log("Found more that one Data Persistence Manager in the scene. Destroying the newest one");
         Destroy(this.gameObject);
         return;
      }

      instance = this;
      DontDestroyOnLoad(this.gameObject);

      if (disableDataPersistence)
      {
         Debug.LogWarning("Data Persistence is currently disabled!");
      }
      
      this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

      this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();

      if (overrideSelectedProfileId)
      {
         this.selectedProfileId = testSelectedProfileId;
         Debug.LogWarning("Overrode selected profile id with test id");
      }
   }

   private void OnEnable()
   {
      SceneManager.sceneLoaded += OnSceneLoaded;
   }
   
   private void OnDisable()
   {
      SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
      Debug.Log("OnSceneLoaded Called");
      this.dataPersistenceObjects = FindAllDataPersistenceObjects();
      LoadGame();
   }

   public void ChangeSelectedProfileId(string newProfileId)
   {
      //update the profile to use for saving and loading
      this.selectedProfileId = newProfileId;
      // load the game which will use that profile, updating our game data accordingly
      LoadGame();
   }

   public void NewGame()
   {
      this.gameData = new GameData();
   }

   public void LoadGame()
   {
      // return right away if data persistence is disabled
      if (disableDataPersistence)
      {
         return;
      }
      
      // Load any saved data from a file using the data handler
      this.gameData = dataHandler.Load(selectedProfileId);
      
      // start a new game if the data is null and we're configured to initialize data for debugging purposes
      if (this.gameData == null && initializeDataIfNull)
      {
         NewGame();
      }
      
      // if no data can be loaded, don't continue
      if (this.gameData == null)
      {
         Debug.Log("No data was found. A New Game needs to be started before data can be loaded");
         return;
      }
      
      // push the loaded data to all other scripts that need it
      foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
      {
         dataPersistenceObj.LoadData(gameData);
      }
      Debug.Log("Loaded location = " + gameData.position);
   }

   public void SaveGame()
   {
      // return right away if data persistence is disabled
      if (disableDataPersistence)
      {
         return;
      }
      
      if (this.gameData == null)
      {
         Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved");
         return;
      }
      
      // pass the data to other script so they can update it
      foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
      {
         dataPersistenceObj.SaveData(gameData);
      }
      Debug.Log("Saved location = " + gameData.position);
      
      // timestamp the data so we know when it was last saved
      gameData.lastUpdated = System.DateTime.Now.ToBinary();
      
      
      // save that data to a file using the data handler
      dataHandler.Save(gameData, selectedProfileId);
   }
   

   private void OnApplicationQuit()
   {
      // SaveGame();
   }

   private List<IDataPersistence> FindAllDataPersistenceObjects()
   {
      //FindObjectsofType takes in an optional boolean to include inactive gameobjects
      
      
      IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
      return new List<IDataPersistence>(dataPersistenceObjects);
   }

   public bool HasGameData()
   {
      return gameData != null;
   }

   public Dictionary<string, GameData> GetAllProfilesGameData()
   {
      return dataHandler.LoadAllProfiles();
   }
}
