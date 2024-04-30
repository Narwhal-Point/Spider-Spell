using UnityEngine;
using Witch.BehaviourTree;

namespace Witch.WitchAI
{
    public class CheckTargetInChaseRange : Node
    {
        private const int EnemyLayerMask = 8;

        private readonly Transform _transform;
        private float _chaseRange;

        public CheckTargetInChaseRange(Transform transform, float chaseRange)
        {
            _transform = transform;
            _chaseRange = chaseRange;
        }

        public override NodeState Evaluate()
        {
            object t = GetData("melee");
            if (t == null)
            {
                Collider[] colliders = Physics.OverlapSphere(
                    _transform.position, _chaseRange, EnemyLayerMask);


                if (colliders.Length > 0)
                {
                    // chase mode, so the ai saves key and the transform of the target for melee mode
                    Parent.Parent.SetData("melee", colliders[0].transform);

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