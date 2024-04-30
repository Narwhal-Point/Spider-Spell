using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Witch.BehaviourTree;
using Sequence = Witch.BehaviourTree.Sequence;

namespace Witch.WitchAI
{
    public class WitchBT : BTree
    {
       private UnityEngine.AI.NavMeshAgent _agent;
        
        // speed of AI
        public float speed = 4f;
        
        // distance from which the enemy will start chasing the player
        public float chaseRange = 6f;

        // distance from which the enemy will start attacking the player with their club
        public float attackRange = 1f;
        
        public Transform[] waypoints;

        private void Start()
        {
            _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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
                // // sequence for chasing
                new Sequence(new List<Node>
                {
                new CheckTargetInChaseRange(transform, chaseRange),
                new WitchChase(transform, chaseRange, speed, _agent),
                }),
                // wandering
                new WitchWander(transform, waypoints, speed, _agent),
            });

            return root;
        }
    }
}
