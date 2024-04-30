using UnityEngine;
using Witch.BehaviourTree;


namespace Witch.WitchAI
{
    // when witch enters this behaviour player is fucked
    public class WitchAttack : Node
    {
        private Transform _lastTarget;

        private float _attackTime = 1f;

        private int _weaponHoldingDegree = 0;

        public WitchAttack()
        {
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("attack");
            // GetComponent is not performant, so only run it when a new target has been found
            if (target != _lastTarget)
            {
                _lastTarget = target;
            }

            ClearData("attack");
            ClearData("target");
            ClearData("retreat");
            ClearData("playerLost");


            State = NodeState.Running;
            return State;
        }
    }
}