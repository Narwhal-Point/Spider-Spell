using System;
using Unity.Mathematics;
using UnityEngine;

namespace Witch
{
    public class WitchTutorialTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject witch;
        private bool active = false;
        private void Start()
        {
            witch.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!active)
            {
                active = true;
                witch.SetActive(true);
            }
        }
    }
}