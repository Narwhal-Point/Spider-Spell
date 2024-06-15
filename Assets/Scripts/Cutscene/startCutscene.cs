using System;
using UI;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class startCutscene : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private SetTextToTextBox continueCutsceneText;
        public bool Started { private get; set;  }

        private void Start()
        {
            continueCutsceneText.SetText("");
        }

        private void Update()
        {
            if (playableDirector.state == PlayState.Paused && Started)
            {
                continueCutsceneText.SetText("Press [Interact] to continue");
                // yes, this is not optimal. idc
                InputManager.instance.EnableInteract();   
                if(InputManager.instance.InteractInput)
                    playableDirector.Resume();
            }
        }
    }
}