using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider boxCollider;

    [SerializeField] private AudioManager audioManager;
    // [SerializeField] AudioSource poofSFX;
    [SerializeField] private float disappearTime = 3f;
    private float disappearTimer = 0f;
    private bool disappearing;
    private float shakeMagnitude = 0.1f; // Adjust the intensity of the shake
    [SerializeField] private float shakeDuration = 3f;  // Adjust the duration of the shake
    private float shakeTimer = 0f;
    private Vector3 shakeStartPosition;
    [SerializeField] ParticleSystem poofVFX;
    private bool playedFX = false;


    // Start is called before the first frame update
    void Start()
    {
        GameObject audioObject = GameObject.Find("AudioManager");
        audioManager = audioObject.GetComponent<AudioManager>();
        shakeStartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (disappearing)
        {
            // Shake effect written with help of ChatGPT)
            if (shakeTimer < shakeDuration)
            {
                shakeTimer += Time.deltaTime;
                float percentComplete = shakeTimer / shakeDuration;
                float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);
                float alpha = Random.Range(0.0f, 1.0f);
                float beta = Random.Range(0.0f, 1.0f);
                Vector3 shakeOffset = new Vector3(Mathf.Lerp(-1.0f, 1.0f, alpha), Mathf.Lerp(-1.0f, 1.0f, beta), 0) * shakeMagnitude * damper;
                transform.position = shakeStartPosition + shakeOffset;
            }

            if (disappearTime - disappearTimer < 0.1 && !playedFX)
            {
                poofVFX.Play();
                audioManager.PlaySFX(audioManager.poofSFX);
                playedFX = true;
            }

            disappearTimer += Time.deltaTime;
            if (disappearTimer > disappearTime)
            {
                //transform.position = new Vector3(0, -100, 0);
                meshRenderer.enabled = false;
                boxCollider.enabled = false;
                disappearing = false;
                Invoke("Reset2Original", 5f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        disappearing = true;
    }

    void Reset2Original()
    {
        //poofVFX.Play();
        audioManager.PlaySFX(audioManager.poofSFX);
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        disappearing = false;
        playedFX = false;
        transform.position = shakeStartPosition;
        disappearTimer = 0f;
        shakeTimer = 0f;
    }
}

