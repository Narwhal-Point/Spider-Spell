using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collectables
{
    public class CollectableManager : MonoBehaviour, IDataPersistence
    {
        private readonly Dictionary<string, GameObject> _collectables = new Dictionary<string, GameObject>();
        public static CollectableManager instance;
        private readonly Dictionary<string, GameObject> _inventory = new Dictionary<string, GameObject>();
        
        private void Awake()
        {
            if(instance == null)
                instance = this;
        }
        
        public void AddToInventory(string key, GameObject collectable)
        {
            if (key == "")
            {
                Debug.LogWarning($"key for {collectable} is empty");
                return;
            }

            if (!collectable)
            {
                Debug.LogWarning($"Gameobject for key {key} is empty");
                return;
            }

            if (!_inventory.TryAdd(key, collectable))
            {
                Debug.LogWarning("collectable already collected.");
                return;
            }

            collectable.SetActive(false);
            ShowCollectedCollectable(key);
        }

        public void AddToCollectables(string key, GameObject sprite)
        {
            if(key == "" || key.Contains("fake"))
                return;
            
            _collectables.TryAdd(key, sprite);
            sprite.transform.GetChild(0).GetComponent<Image>().enabled = false;
            // Debug.Log($"Collectables count: {_collectables.Count}");
        }

        private void ShowCollectedCollectable(string key)
        {
            bool success = _collectables.TryGetValue(key, out var sprite);
            if(success)
                sprite.transform.GetChild(0).GetComponent<Image>().enabled = true;
        }

        // saving and loading name of gameobject because otherwise it doesn't work.
        public void LoadData(GameData data)
        {
            // foreach (var ingredient in data.collectables)
            // {
            //     GameObject o = GameObject.Find(ingredient.Value);
            //     AddToCollectables(ingredient.Key, o);
            // }
            foreach (var ingredient in data.collectedIngredients)
            {
                GameObject o = GameObject.Find(ingredient.Value);
                AddToInventory(ingredient.Key, o);
            }

            foreach (var ingredient in _inventory)
            {
                if (!ingredient.Key.Contains("fake"))
                    _collectables.TryAdd(ingredient.Key, ingredient.Value);
            }
        }

        public void SaveData(GameData data)
        {
            // foreach (var ingredient in _collectables)
            // {
            //     string ingredientName = ingredient.Value.name;
            //     data.collectables.TryAdd(ingredient.Key, ingredientName);
            // }
            
            foreach (var ingredient in _inventory)
            {
                string ingredientName = ingredient.Value.name;
                data.collectedIngredients.TryAdd(ingredient.Key, ingredientName);
            }
        }

        private int GetInventoryCount()
        {
            return _inventory.Count;
        }

        private int GetCollectableCount()
        {
            return _collectables.Count;
        }

        public bool CollectedAll()
        {
            #if UNITY_EDITOR
            Debug.Log($"Collectable Count: {GetCollectableCount()}");
            Debug.Log($"Inventory Count: {GetInventoryCount()}");
            #endif
            return GetCollectableCount() <= GetInventoryCount();
        }
    }
}
