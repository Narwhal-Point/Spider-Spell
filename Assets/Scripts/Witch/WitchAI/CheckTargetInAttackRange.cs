using Witch.BehaviourTree;
using UnityEngine;

namespace Witch.WitchAI
{
    public class CheckTargetInAttackRange : Node
    {
        private const int TargetLayerMask = 8; // wrong value

        private readonly Transform _transform;
        private float _attackRange;

        public CheckTargetInAttackRange(Transform transform, float attackRange)
        {
            _transform = transform;
            _attackRange = attackRange;

        }

        public override NodeState Evaluate()
        {
            
            object t = GetData("Target");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(
                    _transform.position, _attackRange, TargetLayerMask);
                
                
                if (colliders.Length > 0)
                {
                    // chase mode, so the ai saves key and the transform of the target for melee mode
                    Parent.Parent.SetData("Target", colliders[0].transform);
                    
                    State = NodeState.Success;
                    return State;
                }
                
                State = NodeState.Failure;
                return State;
            }

            State = NodeState.Success;
            return State;
        }
    }
}