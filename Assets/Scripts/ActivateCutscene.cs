using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour, IDataPersistence
{
    [SerializeField] private PlayableDirector playableDirector;

    private bool _cutscenePlayed;
    private bool _saved;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !_cutscenePlayed)
        {
            _cutscenePlayed = true;
            playableDirector.Play();
        }
    }

    public void LoadData(GameData data)
    {
        if (playableDirector.name == "Witch Cutscene")
        {
            _cutscenePlayed = data.witchCutscenePlayed;
        }
        else if (playableDirector.name == "Entering Main Room")
        {
            _cutscenePlayed = data.mainRoomCutscenePlayed;
        }
    }

    public void SaveData(GameData data)
    {
        if (_saved)
            return;
        if (_cutscenePlayed)
        {
            if (playableDirector.name == "Witch Cutscene")
            {
                data.witchCutscenePlayed = true;
                _saved = true;
            }
            else if (playableDirector.name == "Entering Main Room")
            {
                data.mainRoomCutscenePlayed = true;
                _saved = true;
            }
        }
    }
}