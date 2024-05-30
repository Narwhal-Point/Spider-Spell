using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Movement;

public class Fan : MonoBehaviour
{
    [SerializeField] private PlayerSwingHandler swingHandler;
    [SerializeField] private GameObject wind;
    [SerializeField] private FanSpin fanSpin;
    [SerializeField] private FanStuck fanStuck;

    private void Start()
    {
        fanSpin.enabled = true;
        fanStuck.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (swingHandler.predictionHit.collider && swingHandler.predictionHit.collider.CompareTag("Fan") && InputManager.instance.FireInput)
        {
            Debug.Log("Shit");
            wind.SetActive(false);
            fanSpin.enabled = false;
            fanStuck.enabled = true;
        }
    }
}
