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
        private readonly float _vibeTime;
        private float _restCounter;
        private readonly float _speed;
        private readonly NavMeshAgent _agent;

        private bool _reachedPoint = false;
        
        

        public WitchWander(Transform transform, Transform[] vibePoints, NavMeshAgent agent, float vibeTime)
        {
            _transform = transform;
            _vibePoints = vibePoints;
            _agent = agent;
            _vibeTime = vibeTime;
            _routeIndex = Random.Range(0, _vibePoints.Length); // Initialize with a random index
            Transform wp = _vibePoints[_routeIndex];
            _agent.destination = wp.position;
        }
        
        public override NodeState Evaluate()
        {
            // wait at patrol point
            if (_isVibin)
            {
                _restCounter += Time.deltaTime;
                if (_restCounter >= _vibeTime)
                    _isVibin = false;
            }
            else
            {
                Transform wp = _vibePoints[_routeIndex];
                // arrived at patrol point
                if (_agent.remainingDistance < 0.01f && !_reachedPoint)
                {
                    _reachedPoint = true;
                    _restCounter = 0f;
                    _isVibin = true;

                    _routeIndex = GetNewRouteIndex(); // Choose a random index
                }
                else
                {
                    // move to patrol point
                    _reachedPoint = false;
                    _agent.destination = wp.position;
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