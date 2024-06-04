using Audio;
using UnityEngine;

namespace Objects
{
    public class PistonManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] pistons;
        [SerializeField] private Vector3 targetDirection;
        private Vector3 _moveDirection;
        [SerializeField] private float maxMoveWidth = 7f;
        private float _currentMoved;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private bool moveFirstGroup = true;
        private bool _movingOut = true;
        private bool _waiting = true;
        [SerializeField] private float waitDuration = 0.5f;
        private float _waitTimer;
        private AudioSource _audioSource;

        // Start is called before the first frame update
        void Start()
        {
            targetDirection = transform.right;
            _audioSource = GetComponent<AudioSource>();
        
            AudioManager.LocationSpecificAudioSource.Add(_audioSource);
        }

        // Update is called once per frame
        void Update()
        {
            if (moveFirstGroup)
            {
                if (pistons.Length > 0)
                {
                    MovePistons(0);
                }
            }
            else
            {
                if (pistons.Length > 1)
                {
                    MovePistons(1);
                }
            }
        }

        private void MovePistons(int startIndex)
        {
            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= waitDuration)
                {
                    _waiting = false;
                    _waitTimer = 0f;
                    _audioSource.Play();
                }
            }
            else
            {
                if (_movingOut)
                {
                    for (int i = startIndex; i < pistons.Length; i += 2)
                    {
                        _moveDirection = targetDirection.normalized * (moveSpeed * Time.deltaTime);
                        pistons[i].transform.position += _moveDirection;
                    }

                    _currentMoved += _moveDirection.magnitude;
                    if (_currentMoved >= maxMoveWidth)
                    {
                        _movingOut = false;
                        _waiting = true;
                        _audioSource.Stop();
                    }
                }

                if (!_movingOut)
                {
                    for (int i = startIndex; i < pistons.Length; i += 2)
                    {
                        _moveDirection = -targetDirection.normalized * (moveSpeed * Time.deltaTime);
                        pistons[i].transform.position += _moveDirection;
                    }

                    _currentMoved -= _moveDirection.magnitude;
                    if (_currentMoved <= 0)
                    {
                        _movingOut = true;
                        moveFirstGroup = !moveFirstGroup;
                        _waiting = true;
                        _audioSource.Stop();
                    }
                }
            }
        }
    }
}