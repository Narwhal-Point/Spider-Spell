using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.UIElements;

public class FanSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000f;
    Vector3 startRotation;
    private float z;
    private AudioSource _audioSource;

    void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
        z = 0.0f;
        _audioSource = GetComponent<AudioSource>();
        AudioManager.LocationSpecificAudioSource.Add(_audioSource);
    }

    // Update is called once per frame
    void Update()
    {
        z += rotationSpeed * Time.deltaTime;
        if (z > 360f)
        {
            z = 0.0f;
        }
        transform.localRotation = Quaternion.Euler(startRotation.x, startRotation.y, z);
    }
    
    private void OnDestroy()
    {
        AudioManager.LocationSpecificAudioSource.Remove(_audioSource);
    }
}
