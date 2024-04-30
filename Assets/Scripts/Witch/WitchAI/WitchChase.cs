using Witch.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;


namespace Witch.WitchAI
{
    public class WitchChase : Node
    {
        private readonly Transform _transform;
        private float _chaseRange;
        private float _speed;
        private NavMeshAgent _agent;

        public WitchChase(Transform transform, float chaseRange, float speed, NavMeshAgent agent)
        {
            _transform = transform;
            _chaseRange = chaseRange;
            _speed = speed;
            _agent = agent;
        }

        
        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("attack");
            
            // move towards target
            if (Vector3.Distance(_transform.position, target.position) > 0.01f)
            {
                _agent.destination = target.position;
                // _transform.position = Vector3.MoveTowards(
                //     _transform.position, target.position, _speed * Time.deltaTime);
                // _transform.LookAt(target.position);
            }

            // is target outside of chase range? Stop chasing
            if (Vector3.Distance(_transform.position, target.position) > _chaseRange)
                ClearData("attack");

            State = NodeState.Running;
            return State;
        }
    }
}