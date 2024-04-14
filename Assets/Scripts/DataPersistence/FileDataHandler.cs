using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
   private string dataDirPath = "";

   private string dataFileName = "";

   public FileDataHandler(string dataDirPath, string dataFileName)
   {
      this.dataDirPath = dataDirPath;
      this.dataFileName = dataFileName;
   }

   public GameData Load(string profileId)
   {
      // base case - if the profileId is null, return right away
      if (profileId == null)
      {
         return null;
      }
      
      string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
      GameData loadedData = null;
      
      if (File.Exists(fullPath))
      {
         try
         {
            // Load the serialized data from the file
            string dataToLoad = "";
            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
               using (StreamReader reader = new StreamReader(stream))
               {
                  dataToLoad = reader.ReadToEnd();
               }
            }
            
            // deserialize data from Json back into C# object
            loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

         }
         catch (Exception e)
         {
            Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
         }
      }
      return loadedData;
   }

   public void Save(GameData data, string profileId)
   {
      // base case - if the profileId is null, return right away
      if (profileId == null)
      {
         return;
      }
      
      string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
      
      try
      {
         //create the directory the file will be written to if it doesn't already exist
         Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
         
         //serialize the C# game data object into Json
         string dataToStore = JsonUtility.ToJson(data, true);
         
         //write the serialized data to the file
         using (FileStream stream = new FileStream(fullPath, FileMode.Create))
         {
            using (StreamWriter writer = new StreamWriter(stream))
            {
               writer.Write(dataToStore);
            }
         } 

      }
      catch (Exception e)
      {
         Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
      }
   }

   public Dictionary<string, GameData> LoadAllProfiles()
   {
      Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

      IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
      foreach (DirectoryInfo directoryInfo in dirInfos)
      {
         string profileId = directoryInfo.Name;
         // check if data files exist, if it doesnt then it isn't a profile and should be skipped
         string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
         if (!File.Exists(fullPath))
         {
            Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId);
            continue;
         }
         //load the game data for this profile and put it in the dictionary and check if data isn't null
         GameData profileData = Load(profileId);
         if (profileData != null)
         {
            profileDictionary.Add(profileId, profileData);
         }
         else
         {
            Debug.LogError("Tried to load profile but something went wrong for profile:" + profileId);
         }
         
      }

      return profileDictionary;
   }

   public string GetMostRecentlyUpdatedProfileId()
   {
      string mostRecentProfileId = null;
      
      Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
      foreach (KeyValuePair<string, GameData> pair in profilesGameData)
      {
         //skip entry if gamedata is null
         string profileId = pair.Key;
         GameData gameData = pair.Value;

         if (gameData == null)
         {
            continue;
         }
         // if this is the first data, its the most recent
         if (mostRecentProfileId == null)
         {
            mostRecentProfileId = profileId;
         }
         // other wise compare profile dates
         else
         {
            DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
            DateTime newDateTime = DateTime.FromBinary((gameData.lastUpdated));
            if (newDateTime > mostRecentDateTime)
            {
               mostRecentProfileId = profileId;
            }
         }
      }
      return mostRecentProfileId;
   }
}
