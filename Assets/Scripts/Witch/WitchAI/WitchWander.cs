using UnityEngine;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using Witch.BehaviourTree;

namespace Witch.WitchAI
{
    public class WitchWander : Node
    {
        private readonly Transform _transform;
        private readonly Transform[] _vibePoints;
        
        private int _routeIndex;
        
        private bool _isVibin;
        private const float VibeTime = 1f;
        private float _restCounter;
        private readonly float _speed;
        private readonly NavMeshAgent _agent;
        
        

        public WitchWander(Transform transform, Transform[] vibePoints, NavMeshAgent agent)
        {
            _transform = transform;
            _vibePoints = vibePoints;
            _routeIndex = GetNewRouteIndex(); // Initialize with a random index
            _agent = agent;

        }
        
        public override NodeState Evaluate()
        {
            // wait at patrol point
            if (_isVibin)
            {
                _restCounter += Time.deltaTime;
                if (_restCounter >= VibeTime)
                    _isVibin = false;
            }
            else
            {
                Transform wp = _vibePoints[_routeIndex];
                // arrived at patrol point
                if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
                {
                    _transform.position = wp.position;
                    _restCounter = 0f;
                    _isVibin = true;

                    _routeIndex = GetNewRouteIndex(); // Choose a random index
                }
                else
                {
                    _agent.destination = wp.position;
                    // move to patrol point
                    // _transform.position = Vector3.MoveTowards(_transform.position, wp.position, _speed * Time.deltaTime);
                    // _transform.LookAt(wp.position);
                }
            }

            State = NodeState.Running;
            return State;
        }

        private int GetNewRouteIndex()
        {
            
            int newIndex;
            do
            {
                newIndex = Random.Range(0, _vibePoints.Length);
            } while (newIndex == _routeIndex);

            return newIndex;
        }
    }
}