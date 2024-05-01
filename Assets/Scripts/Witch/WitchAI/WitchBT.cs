using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Witch.BehaviourTree;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace Witch.WitchAI
{
    public class WitchBT : BTree
    {
        private NavMeshAgent _agent;
        private WitchFov _fov;

        // [Tooltip("Select the layers the witch should try to chase and attack")]
        // public LayerMask targetLayer;

        // [Tooltip("Range from which the witch will start chasing the player")]
        // public float chaseRange = 6f;

        [Tooltip("distance from which the witch will attack the player")]
        public float attackRange = 1f;

        [Space] [Tooltip("amount of time the witch rests at the waypoints")]
        public float restTime = 1f;

        public Transform[] waypoints;

        private new void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _fov = GetComponent<WitchFov>();
            // Debug.Log(targetLayer.value);
            base.Start();
        }

        protected override Node SetupTree()
        {
            // start with selector
            Node root = new Selector(new List<Node>
            {
                // sequence for attack
                new Sequence(new List<Node>
                {
                    new CheckTargetInAttackRange(transform, attackRange, _fov.targetMask),
                    new WitchAttack(),
                }),
                // sequence for chasing
                new Sequence(new List<Node>
                {
                    new CheckTargetInChaseRange(transform, _fov),
                    new WitchChase(transform, _agent, _fov),
                }),
                // wandering
                new WitchWander(transform, waypoints, _agent, restTime),
            });

            return root;
        }
    }
}