using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Witch.BehaviourTree;
using Sequence = Witch.BehaviourTree.Sequence;
using Tree = Witch.BehaviourTree.Tree;

namespace Witch.WitchAI
{
    public class WitchBT : Tree
    {
        private void Start()
        {
            
        }

        // speed of AI
        public const float Speed = 4f;

        // distance from which the enemy starts shooting
        public const float FOVRange = 10f;

        // distance from which the enemy will start retreating from the player
        public const float RetreatRange = 8f;

        // distance from which the enemy will start chasing from the player
        public const float ChaseRange = 6f;

        // distance from which the enemy will start attacking the player with their club
        public const float AttackRange = 1f;

        protected override Node SetupTree()
        {
            // start with selector
            Node root = new Selector(new List<Node>
            {
                // sequence for melee
                new Sequence(new List<Node>
                {
                    new CheckTargetInAttackRange(transform),
                    // new EnemyMelee(meleeWeapon),
                }),
                // // sequence for chasing
                // new Sequence(new List<Node>
                // {
                //     new CheckTargetInChaseRange(transform),
                //     new EnemyChase(transform),
                // }),
                // // sequence for retreating
                // new Sequence(new List<Node>
                // {
                //     new CheckTargetInRetreatRange(transform),
                //     new EnemyRetreat(transform),
                // }),
                // // sequence for shooting
                // new Sequence(new List<Node>
                // {
                //     new CheckTargetInShootRange(transform, gun),
                //     new EnemyShoot(transform, projectile),
                // }),
                // // searching
                // new EnemySearch(transform),
                // // patrolling
                // new EnemyPatrol(transform, waypoints),
            });

            return root;
        }
    }
}
