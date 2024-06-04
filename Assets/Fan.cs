using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Movement;
using static UnityEngine.InputManagerEntry;

public class Fan : MonoBehaviour
{
    [SerializeField] private PlayerSwingHandler swingHandler;
    [SerializeField] private GameObject[] winds;
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
        if (swingHandler.predictionHit.collider && swingHandler.predictionHit.collider.gameObject == gameObject &&
            InputManager.instance.FireInput)
        {
            Debug.Log("Shit");
            for (int i = 0; i < winds.Length; i++)
            {
                winds[i].SetActive(false);
            }
            fanSpin.enabled = false;
            fanStuck.enabled = true;
        }
    }
}