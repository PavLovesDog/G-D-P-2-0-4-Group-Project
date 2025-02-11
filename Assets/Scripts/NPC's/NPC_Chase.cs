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
        GameManager gameManager;
        EnemyStats enemyStats;
        PlayerMovement player;

        [Header("Chase variables")]
        public float chaseSpeed = 5;
        public float patrolSpeed = 3.5f;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyManager = GetComponent<EnemyManager>();
            enemyStats = GetComponent<EnemyStats>();
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<PlayerMovement>();
        }

        void Update()
        {
            if (!gameManager.gamePaused)
            {
                if (enemyManager.foundPlayer && !player.isDead && !enemyManager.isDead && enemyManager.player != null)
                {
                    navMeshAgent.speed = chaseSpeed;
                    if (!enemyStats.hasHit) // this is so enemy waits a litlle bit after each hit
                        navMeshAgent.SetDestination(enemyManager.player.transform.position);
                }
                else
                {
                    //reset speed
                    if(navMeshAgent != null)
                        navMeshAgent.speed = patrolSpeed;
                }
            }
        }
    }
}
