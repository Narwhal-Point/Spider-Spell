using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    
    [Header("Audio Clip")]
    public AudioClip background;
    
    [SerializeField] private AudioMixer myMixer;


    // private void Awake()
    // {
    //     if (PlayerPrefs.HasKey("musicVolume"))
    //     {
    //         float volume = PlayerPrefs.GetFloat("musicVolume");
    //         myMixer.SetFloat("music", Mathf.Log10(volume)*20);
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("musicVolume");
            myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        }
        musicSource.clip = background;
        musicSource.Play();
    }
}
