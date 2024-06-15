using UnityEngine;
using UnityEngine.UI;

namespace Collectables
{
    public class Collectable : MonoBehaviour
    {
        public string id = "";

        [SerializeField] private float rotationSpeed = 50f; // Speed of rotation
        [SerializeField] private float bopSpeed = 0.5f; // Speed of bopping
        [SerializeField] private float bopHeight = 0.2f; // Height of bopping
        [SerializeField] private GameObject image;
        
        private Vector3 _startPosition;

        private void OnTriggerEnter(Collider other)
        {
            CollectableManager.instance.AddToInventory(id, gameObject);
        }

        private void Start()
        {
            CollectableManager.instance.AddToCollectables(id, image);
            
            // Save the initial position
            _startPosition = transform.position;
        }

        private void Update()
        {
            // Rotate the object around its Y axis
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

            // Move the object up and down
            float newY = _startPosition.y + Mathf.Sin(Time.time * bopSpeed) * bopHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}