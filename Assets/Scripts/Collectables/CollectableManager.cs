using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collectables
{
    public class CollectableManager : MonoBehaviour, IDataPersistence
    {
        private readonly Dictionary<string, Image> _collectables = new Dictionary<string, Image>();
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

        private GameObject GetObjectFromInventory(string key)
        {
            _inventory.TryGetValue(key, out var collectable);
            return collectable;
        }

        private bool RemoveFromInventory(string key)
        {
            return _inventory.Remove(key);
        }

        public void AddToCollectables(string key, Image sprite)
        {
            if(key == "")
                return;
            
            _collectables.Add(key, sprite);
            sprite.enabled = false;
            // Debug.Log($"Collectables count: {_collectables.Count}");
        }

        private void ShowCollectedCollectable(string key)
        {
            bool success = _collectables.TryGetValue(key, out var sprite);
            if(success)
                sprite.enabled = true;
        }

        // saving and loading name of gameobject because otherwise it doesn't work.
        public void LoadData(GameData data)
        {
            foreach (var ingredient in data.collectedIngredients)
            {
                GameObject o = GameObject.Find(ingredient.Value);
                AddToInventory(ingredient.Key, o);
            }
        }

        public void SaveData(GameData data)
        {
            foreach (var ingredient in _inventory)
            {
                string ingredientName = ingredient.Value.name;
                data.collectedIngredients.TryAdd(ingredient.Key, ingredientName);
            }
        }
    }
}
