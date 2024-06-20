using System;
using UI;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class startCutscene : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        public bool Started { private get; set; }

        private void Update()
        {
            if (playableDirector.state == PlayState.Paused && Started)
            {
                // yes, this is not optimal. idc
                InputManager.instance.EnableInteract();
                if (InputManager.instance.InteractInput)
                    playableDirector.Resume();
            }
        }
    }
}