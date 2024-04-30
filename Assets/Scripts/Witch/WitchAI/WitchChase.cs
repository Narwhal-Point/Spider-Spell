using Witch.BehaviourTree;
using UnityEngine;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;


namespace Witch.WitchAI
{
    public class WitchChase : Node
    {
        private readonly Transform _transform;
        private float _chaseRange;
        private NavMeshAgent _agent;

        public WitchChase(Transform transform, float chaseRange, NavMeshAgent agent)
        {
            _transform = transform;
            _chaseRange = chaseRange;
            _agent = agent;
            Debug.Log(_agent);
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
            if (Vector3.Distance(_transform.position, target.position) > 0.01f)
            {
                _agent.destination = target.position;
            }

            // is target outside of chase range? Stop chasing
            if (Vector3.Distance(_transform.position, target.position) > _chaseRange)
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