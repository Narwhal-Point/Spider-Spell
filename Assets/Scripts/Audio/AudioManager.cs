using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;

    [SerializeField] public AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip background;
    public AudioClip walking;
    public AudioClip landing;
    public AudioClip webshooting;
    public AudioClip jumping;
    public AudioClip WitchAppearTutorial;
    
    
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    
    public void PlayLoopSFX(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.loop = true;
        SFXSource.pitch = 1.65f;
        SFXSource.Play();
    }
    
    public void StopSFX()
    {
        SFXSource.loop = false;
        SFXSource.pitch = 1;
        SFXSource.Stop();
    }
}
