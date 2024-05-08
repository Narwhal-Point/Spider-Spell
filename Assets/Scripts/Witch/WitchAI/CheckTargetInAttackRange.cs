using Witch.BehaviourTree;
using UnityEngine;

namespace Witch.WitchAI
{
    public class CheckTargetInAttackRange : Node
    {
        private readonly int _targetLayerMask;

        private readonly Transform _transform;
        private readonly float _attackRange;

        public CheckTargetInAttackRange(Transform transform, float attackRange, int targetLayerMask)
        {
            _transform = transform;
            _attackRange = attackRange;
            _targetLayerMask = targetLayerMask;

        }

        public override NodeState Evaluate()
        {
            
            object t = GetData("attack");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(
                    _transform.position, _attackRange, _targetLayerMask);
                
                
                if (colliders.Length > 0)
                {
                    // chase mode, so the ai saves key and the transform of the target for melee mode
                    Parent.Parent.SetData("attack", colliders[0].transform);
                    
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