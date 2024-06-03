using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Source")] [SerializeField]
        public AudioSource musicSource;

        // turn audio source into array, so we have multiple.
        [FormerlySerializedAs("SFXSources")] [SerializeField] public AudioSource[] sfxSources;

        [Header("Audio Clip")] public AudioClip background;
        public AudioClip walking;
        public AudioClip landing;
        public AudioClip webshooting;
        public AudioClip jumping;
        [FormerlySerializedAs("WitchAppearTutorial")] public AudioClip witchAppearTutorial;
        [FormerlySerializedAs("poofSFX")] public AudioClip poofSfx;
        [FormerlySerializedAs("checkpointSFX")] public AudioClip checkpointSfx;
        public AudioClip stoneShoving;
    
        // some sounds need to be on a specific location. To still pause them we add them to this list.
        public static readonly List<AudioSource> LocationSpecificAudioSource = new List<AudioSource>();


        private void Start()
        {
            musicSource.clip = background;
            musicSource.Play();
        }

        private void Update()
        {
            if (Time.timeScale == 0)
            {
                foreach (var sfxSource in sfxSources)
                {
                    sfxSource.Pause();
                }

                foreach (var audioSource in LocationSpecificAudioSource)
                {
                    audioSource.Pause();
                }
            }
            if (Time.timeScale != 0)
            {
                foreach (var sfxSource in sfxSources)
                {
                    sfxSource.UnPause();
                }

                foreach (var audioSource in LocationSpecificAudioSource)
                {
                    audioSource.UnPause();
                }
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            // loop through the sounds. When an empty one is found play the sound.
            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxSource.PlayOneShot(clip);
                    break;
                }
            }
        }

        public void PlayLoopSFX(AudioClip clip)
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            // loop through the sounds. When an empty one is found play the sound.
            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxSource.clip = clip;
                    sfxSource.loop = true;
                    sfxSource.pitch = 1.65f;
                    sfxSource.Play();
                    break;
                }
            }
        }

        public void StopSFX(AudioClip clip)
        {
            // loop through the sounds. When the one that corresponds to the audio clip is found stop playing.
            foreach (var sfxSource in sfxSources)
            {
                if (sfxSource.clip == clip)
                {
                    sfxSource.loop = false;
                    sfxSource.pitch = 1;
                    sfxSource.Stop();
                }
            }
        }
    }
}