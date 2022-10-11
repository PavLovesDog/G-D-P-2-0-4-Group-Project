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

        [Header("Chase variables")]
        public float chaseSpeed = 5;
        public float patrolSpeed = 3.5f;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyManager = GetComponent<EnemyManager>();
        }

        void Update()
        {
            if (enemyManager.foundPlayer && !enemyManager.isDead && enemyManager.player != null)
            {
                navMeshAgent.speed = chaseSpeed;
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
