using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Cutscene
{
    public class ActivateCutscene : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private bool delayedStart;
        [Tooltip("The delay in seconds for when delayed Start is enabled")]
        [SerializeField] private float delay;

        private bool _cutscenePlayed;
        private bool _saved;

        private void OnTriggerEnter(Collider collision)
        {
            if(_cutscenePlayed)
                return;
        
            if(delayedStart && collision.CompareTag("Player"))
            {
                _cutscenePlayed = true;
                StartCoroutine(DelayStart());
            }
            else if (collision.CompareTag("Player"))
            {
                _cutscenePlayed = true;
                playableDirector.Play();
            }
        }

        private IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(delay);
        
            playableDirector.Play();
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
            else if (playableDirector.name == "Start cutscene")
            {
                _cutscenePlayed = data.introCutscenePlayed;
            }
        
            // no need for script. Destroy
            if(_cutscenePlayed)
                Destroy(this);
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
                if (playableDirector.name == "Entering Main Room")
                {
                    data.mainRoomCutscenePlayed = true;
                    _saved = true;
                }
                if (playableDirector.name == "Start cutscene")
                {
                    data.introCutscenePlayed = true;
                    _saved = true;
                }
            }
        }
    }
}