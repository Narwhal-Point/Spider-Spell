using UnityEngine;
using Witch.BehaviourTree;

namespace Witch.WitchAI
{
    public class CheckTargetInChaseRange : Node
    {

        private readonly Transform _transform;
        private WitchFov _fov;

        public CheckTargetInChaseRange(Transform transform, WitchFov fov)
        {
            _transform = transform;
            _fov = fov;
        }

        public override NodeState Evaluate()
        {
            object t = GetData("target");
            if (t == null)
            {
                if (_fov.CanSeePlayer)
                {
                    Parent.Parent.SetData("target", _fov.PlayerRef.transform);
                    State = NodeState.Success;
                    return State;
                }
                
                State = NodeState.Failure;
                return State;
                // Collider[] colliders = Physics.OverlapSphere(
                //     _transform.position, _chaseRange, _enemyLayerMask);
                //
                //
                // if (colliders.Length > 0)
                // {
                //     // chase mode, so the ai saves key and the transform of the target for melee mode
                //     Parent.Parent.SetData("target", colliders[0].transform);
                //
                //     State = NodeState.Success;
                //     return State;
                // }
                //
                // State = NodeState.Failure;
                // return State;
            }

            State = NodeState.Success;
            return State;
        }
    }
}