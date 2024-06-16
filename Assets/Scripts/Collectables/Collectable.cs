using System;
using UnityEngine;
using UnityEngine.UI;

namespace Collectables
{
    public class Collectable : MonoBehaviour
    {
        [Tooltip("whatever you do. Do not put 'fake in the id of a collectable'")]
        public string id = "";

        [SerializeField] private float rotationSpeed = 50f; // Speed of rotation
        [SerializeField] private float bopSpeed = 0.5f; // Speed of bopping
        [SerializeField] private float bopHeight = 0.2f; // Height of bopping
        [SerializeField] private GameObject image;

        [Tooltip("check this if the collectable should not count towards the total necessary to finish the game")]
        [SerializeField] private bool fakeCollectable;

#if UNITY_EDITOR
        [SerializeField] private bool debugCollect;
#endif

        private Vector3 _startPosition;

        private void OnTriggerEnter(Collider other)
        {
            CollectableManager.instance.AddToInventory(id, gameObject);
        }

        private void Start()
        {
            // add fake to id so it can't be added to the collectables
            if (fakeCollectable)
            {
                id += "fake";
                Debug.Log("fake id: " + id);

            }
            else
                CollectableManager.instance.AddToCollectables(id, image);

            // Save the initial position
            _startPosition = transform.position;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (debugCollect)
                CollectableManager.instance.AddToInventory(id, gameObject);
#endif

            // Rotate the object around its Y axis
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // Move the object up and down
            float newY = _startPosition.y + Mathf.Sin(Time.time * bopSpeed) * bopHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}