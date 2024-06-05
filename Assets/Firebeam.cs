using Player;
using Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebeam : MonoBehaviour
{
    [SerializeField] float maxHeightMultiplier = 2;
    [SerializeField] float riseSpeed = 3;
    [SerializeField] float upTime = 5;
    [SerializeField] float downTime = 5;
    [SerializeField] float maxDissolve = 0.75f;
    [SerializeField] float minDissolve = 0.87f;
    [SerializeField] MeshRenderer fire;

    private Vector3 startingScale;
    private float timer;
    private bool goingUp;
    private bool goingDown;
    private bool stayUp;
    private bool coolingDown;
    private float initialHeight;
    private float targetHeight;

    // Start is called before the first frame update
    void Start()
    {
        goingUp = false;
        goingDown = false;
        stayUp = false;
        coolingDown = true;
        timer = 0;
        startingScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        initialHeight = startingScale.y;
        targetHeight = initialHeight * maxHeightMultiplier;
        fire.material.SetFloat("_Dissolve", minDissolve);
    }

    // Update is called once per frame
    void Update()
    {
        if (goingUp)
        {
            float riseValue = riseSpeed * Time.deltaTime;
            transform.localScale = new Vector3(startingScale.x, transform.localScale.y + riseValue, startingScale.z);

            float progress = (transform.localScale.y - initialHeight) / (targetHeight - initialHeight);
            float dissolveValue = Mathf.Lerp(minDissolve, maxDissolve, progress);
            fire.material.SetFloat("_Dissolve", dissolveValue);

            if (transform.localScale.y / initialHeight >= maxHeightMultiplier)
            {
                goingUp = false;
                stayUp = true;
            }
        }
        else if (stayUp)
        {
            timer += Time.deltaTime;
            if (timer > upTime)
            {
                stayUp = false;
                goingDown = true;
                timer = 0;
            }
        }
        else if (goingDown)
        {
            float fallValue = riseSpeed * Time.deltaTime;
            transform.localScale = new Vector3(startingScale.x, transform.localScale.y - fallValue, startingScale.z);

            float progress = (transform.localScale.y - initialHeight) / (targetHeight - initialHeight);
            float dissolveValue = Mathf.Lerp(minDissolve, maxDissolve, progress);
            fire.material.SetFloat("_Dissolve", dissolveValue);

            if (transform.localScale.y <= initialHeight)
            {
                goingDown = false;
                coolingDown = true;
                transform.localScale = new Vector3(startingScale.x, initialHeight, startingScale.z);
            }
        }
        else if (coolingDown)
        {
            timer += Time.deltaTime;
            if (timer > downTime)
            {
                coolingDown = false;
                goingUp = true;
                timer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("shit");
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("blowing");
            PlayerDeathManager deathManager = other.gameObject.GetComponent<PlayerDeathManager>();
            deathManager.KillPlayer();
        }
    }
}
