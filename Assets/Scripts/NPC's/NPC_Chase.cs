using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MBF
{
    public class NPC_Chase : MonoBehaviour
    {
        NavMeshAgent navMeshAgent;
        EnemyManager enemyManager;
        EnemyStats enemyStats;

        [Header("Chase variables")]
        public float chaseSpeed = 5;
        public float patrolSpeed = 3.5f;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyManager = GetComponent<EnemyManager>();
            enemyStats = GetComponent<EnemyStats>();
        }

        void Update()
        {
            if (enemyManager.foundPlayer && !enemyManager.isDead && enemyManager.player != null)
            {
                navMeshAgent.speed = chaseSpeed;
                if(!enemyStats.hasHit)
                    navMeshAgent.SetDestination(enemyManager.player.transform.position);
            }
            else
            {
                //reset speed
                navMeshAgent.speed = patrolSpeed;
            }
        }
    }
}
