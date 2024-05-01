using UnityEngine;
using Witch.BehaviourTree;


namespace Witch.WitchAI
{
    // when witch enters this behaviour player is fucked
    public class WitchAttack : Node
    {
        private Transform _lastTarget;

        public WitchAttack()
        {
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            // GetComponent is not performant, so only run it when a new target has been found
            if (target != _lastTarget)
            {
                _lastTarget = target;
            }
            Debug.Log("defeated player");
            
            ClearData("target");

            State = NodeState.Failure;
            return State;
        }
    }
}