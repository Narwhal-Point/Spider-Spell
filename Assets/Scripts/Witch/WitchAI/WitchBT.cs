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
        
        // distance from which the enemy will start chasing the player
        [Tooltip("Range from which the witch will start chasing the player")]
        public float chaseRange = 6f;

        // distance from which the enemy will start attacking the player with their club
        public float attackRange = 1f;
        
        // amount of time the witch rests at the waypoints
        public float restTime = 1f;
        
        public Transform[] waypoints;

        private new void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
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
                    new CheckTargetInAttackRange(transform, attackRange),
                    new WitchAttack(),
                }),
                // sequence for chasing
                new Sequence(new List<Node>
                {
                new CheckTargetInChaseRange(transform, chaseRange),
                new WitchChase(transform, chaseRange, _agent),
                }),
                // wandering
                new WitchWander(transform, waypoints, _agent, restTime),
            });

            return root;
        }
    }
}
