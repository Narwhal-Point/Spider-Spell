using Player;
using UnityEngine;

namespace Objects
{
    public class Firebeam : MonoBehaviour
    {
        [SerializeField] private float maxHeightMultiplier = 2;
        [SerializeField] private float riseSpeed = 3;
        [SerializeField] private float upTime = 5;
        [SerializeField] private float downTime = 5;
        [SerializeField] private float calmGravity = -3;
        [SerializeField] private float heavyGravity = -20;
        [SerializeField] private ParticleSystem fireVFX;

        private Vector3 _startingScale;
        private float _timer;
        private bool _goingUp;
        private bool _goingDown;
        private bool _stayUp;
        private bool _coolingDown;
        private float _initialHeight;

        // Start is called before the first frame update
        private void Start()
        {
            _goingUp = false;
            _goingDown = false;
            _stayUp = false;
            _coolingDown = true;
            _timer = 0;
            _startingScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _initialHeight = _startingScale.y;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_goingUp)
            {
                float riseValue = riseSpeed * Time.deltaTime;
                transform.localScale = new Vector3(_startingScale.x, transform.localScale.y + riseValue, _startingScale.z);

                if (transform.localScale.y / _initialHeight >= maxHeightMultiplier)
                {
                    _goingUp = false;
                    _stayUp = true;
                }
            }
            else if (_stayUp)
            {
                _timer += Time.deltaTime;
                if (_timer > upTime)
                {
                    _stayUp = false;
                    _goingDown = true;
                    fireVFX.gravityModifier = calmGravity;
                    _timer = 0;
                }
            }
            else if (_goingDown)
            {
                float fallValue = riseSpeed * Time.deltaTime;
                transform.localScale = new Vector3(_startingScale.x, transform.localScale.y - fallValue, _startingScale.z);

                if (transform.localScale.y <= _initialHeight)
                {
                    _goingDown = false;
                    _coolingDown = true;
                    transform.localScale = new Vector3(_startingScale.x, _initialHeight, _startingScale.z);
                }
            }
            else if (_coolingDown)
            {
                _timer += Time.deltaTime;
                if (_timer > downTime)
                {
                    _coolingDown = false;
                    _goingUp = true;
                    fireVFX.gravityModifier = heavyGravity;
                    _timer = 0;
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
}