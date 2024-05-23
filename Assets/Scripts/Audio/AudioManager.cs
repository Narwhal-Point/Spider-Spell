using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")] [SerializeField]
    public AudioSource musicSource;

    // turn audio source into array, so we have multiple.
    [SerializeField] public AudioSource[] SFXSources;

    [Header("Audio Clip")] public AudioClip background;
    public AudioClip walking;
    public AudioClip landing;
    public AudioClip webshooting;
    public AudioClip jumping;
    public AudioClip WitchAppearTutorial;
    public AudioClip poofSFX;
    public AudioClip checkpointSFX;


    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        // loop through the sounds. When an empty one is found play the sound.
        foreach (var SFXSource in SFXSources)
        {
            if (!SFXSource.isPlaying)
            {
                SFXSource.PlayOneShot(clip);
                break;
            }
        }
    }

    public void PlayLoopSFX(AudioClip clip)
    {
        // loop through the sounds. When an empty one is found play the sound.
        foreach (var SFXSource in SFXSources)
        {
            if (!SFXSource.isPlaying)
            {
                SFXSource.clip = clip;
                SFXSource.loop = true;
                SFXSource.pitch = 1.65f;
                SFXSource.Play();
                break;
            }
        }
    }

    public void StopSFX(AudioClip clip)
    {
        // loop through the sounds. When the one that corresponds to the audio clip is found stop playing.
        foreach (var SFXSource in SFXSources)
        {
            if (SFXSource.clip == clip)
            {
                SFXSource.loop = false;
                SFXSource.pitch = 1;
                SFXSource.Stop();
            }
        }
    }
}