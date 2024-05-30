using UnityEngine;

namespace Witch
{
    public class WitchTutorialTrigger : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private AudioManager audio;
        [SerializeField] private GameObject witch;
        [SerializeField] private float witchWaitTime = 8f;
        private float _timer;
        private bool _active;

        private bool _cutscenePlayed;
        
        

        private void Start()
        {
            witch.SetActive(false);

            if (_cutscenePlayed)
            {
                Destroy(witch);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_cutscenePlayed)
                return;
            
            if (!_active)
            {
                audio.PlaySFX(audio.WitchAppearTutorial);
                _active = true;
                witch.SetActive(true);
            }
        }

        private void Update()
        {
            if (_active)
            {
                _timer += Time.deltaTime;
                if (_timer > witchWaitTime)
                {
                    witch.SetActive(false);
                }
            }
        }

        public void LoadData(GameData data)
        {
            _cutscenePlayed = data.witchCutscenePlayed;
        }

        public void SaveData(GameData data)
        {
        }
    }
}