using Witch.BehaviourTree;
using UnityEngine;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;


namespace Witch.WitchAI
{
    public class WitchChase : Node
    {
        private readonly Transform _transform;
        private NavMeshAgent _agent;
        private WitchFov _fov;
        private float _attackRange;

        public WitchChase(Transform transform, NavMeshAgent agent, WitchFov fov, float attackRange)
        {
            _transform = transform;
            _agent = agent;
            _fov = fov;
            _attackRange = attackRange;
        }

        
        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            
            if(!target)
            {
                State = NodeState.Failure;
                return State;
            }
            
            // move towards target
            if (Vector3.Distance(_transform.position, target.position) > _attackRange)
            {
                _agent.destination = target.position;
            }
            else
            {
                State = NodeState.Failure;
                return State;
            }

            // is target outside of chase range? Stop chasing
            if (!_fov.CanSeePlayer)
            {
                ClearData("target");
                State = NodeState.Failure;
                return State;
            }

            State = NodeState.Running;
            return State;
        }
    }
}