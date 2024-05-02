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
            Transform target = (Transform)GetData("attack");

            Debug.Log("defeated player");

            ClearData("target");
            ClearData("attack");

            State = NodeState.Success;
            return State;
        }
    }
}