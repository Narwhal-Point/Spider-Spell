using UnityEngine;
using Witch.BehaviourTree;

namespace Witch.WitchAI
{
    public class CheckTargetInChaseRange : Node
    {
        private readonly int _enemyLayerMask;

        private readonly Transform _transform;
        private readonly float _chaseRange;

        public CheckTargetInChaseRange(Transform transform, float chaseRange, int enemyLayerMask)
        {
            _transform = transform;
            _chaseRange = chaseRange;
            _enemyLayerMask = enemyLayerMask;
        }

        public override NodeState Evaluate()
        {
            object t = GetData("target");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(
                    _transform.position, _chaseRange, _enemyLayerMask);


                if (colliders.Length > 0)
                {
                    // chase mode, so the ai saves key and the transform of the target for melee mode
                    Parent.Parent.SetData("target", colliders[0].transform);

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