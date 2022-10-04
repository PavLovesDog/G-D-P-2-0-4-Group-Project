using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MBF
{
    public class NPC_Chase : MonoBehaviour
    {
        [SerializeField]
        public GameObject player;

        NPCDynamicPatrol npcPatrol;
        NavMeshAgent navMeshAgent;

        [Header("Player Detection")]
        public float distanceToPlayer;
        public float detectionRadius;
        public float currentDetectionRadius;
        public float chaseSpeed = 5;
        public bool foundPlayer;

        [Header("Patrol Variables")]
        public float patrolSpeed = 3.5f;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            npcPatrol = GetComponent<NPCDynamicPatrol>();
            player = GameObject.FindGameObjectWithTag("Player");
            currentDetectionRadius = detectionRadius;
        }

        void Update()
        {

            if (navMeshAgent == null)
            {
                Debug.LogError("Nav Mesh Agent Not attached to " + gameObject.name);
            }
            else
            {
                Vector3 targetDestination = player.transform.position; // create the target destination based off destination transform
                distanceToPlayer = Vector3.Distance(this.transform.position, targetDestination); //find distance between object and target
                // check distance to player and adjust bool
                if (distanceToPlayer < currentDetectionRadius)
                {
                    foundPlayer = true;
                }
                else
                {
                    foundPlayer = false;
                }
                
                if (!npcPatrol.isPersuing)
                {
                    //chase target
                    SetDestination();
                }
                else
                {
                    //reset speed
                    navMeshAgent.speed = patrolSpeed;
                }
            }
        }

        public void SetDestination()
        {

            if (player != null && foundPlayer) // if destination exists
            {
                //RaycastHit hit;
                //Debug.DrawLine(this.transform.position, targetDestination, Color.red, distanceToPlayer);
                //if (distanceToPlayer < detectionRadius) // if within range
                //{
                //    // IF THERE IS SOMETHING BLOCKING VIEW
                //    if (Physics.Raycast(this.transform.position, player.position, out hit, distanceToPlayer))
                //    {
                //        //something blocking view, do nothing
                //    }
                //    else
                //    {
                //        foundPlayer = true;
                //        // set speed chase
                //    }
                //}
                //else
                //{
                //    foundPlayer = false;
                //    //npcPatrol.isWaiting = true;
                //    //nPCPatrol.waitTimer = 5.0f;
                //    //randomize destination position, emulate walk
                //}

                navMeshAgent.SetDestination(player.transform.position); // make navemesh agent attach to script move to location
                navMeshAgent.speed = chaseSpeed;
            }
            else
            {
                //navMeshAgent.speed = patrolSpeed;
            }
        }
    }
}
