using System;
using Audio;
using Unity.Mathematics;
using UnityEngine;

namespace Witch
{
    public class WitchTutorialTrigger : MonoBehaviour
    {
        [SerializeField] private AudioManager audio;
        [SerializeField] private GameObject witch;
        [SerializeField] private float witchWaitTime = 8f;
        private float _timer;
        private bool _active = false;
        private void Start()
        {
            witch.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!_active)
            {
                audio.PlaySFX(audio.witchAppearTutorial);
                _active = true;
                witch.SetActive(true);
                
            }
        }

        private void Update()
        {
            if (_active)
            {
                _timer += Time.deltaTime;
                if (_timer > witchWaitTime)
                {
                    witch.SetActive(false);
                }
            }
        }
        
    }
}