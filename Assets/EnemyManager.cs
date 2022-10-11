using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("State Bools")]
        public bool isDead; // handle moving states
        public bool isPatrolling; // handle when the enemy will chase the player, or patrol
        public bool playerSneaking; // will trigger different search mechanics for enemy

        [SerializeField]
        public GameObject player;

        [Header("Player Detection")]
        public float distanceToPlayer;
        public float sphereDetectionRadius;
        public float currentSphereDetectionRadius;
        public bool foundPlayer;

        [Header("Raycast Variables")]
        public GameObject currentHitObject;
        Vector3 origin;
        Vector3 direction;
        public LayerMask layerMask; // set in inspector for "Controller" i.e the player
        public float rayLineDetectionRadius;
        public float rayDetectionDistance;
        public float currentRayDetectionDistance;


        private void Start()
        {
            //start bools
            isDead = false;
            isPatrolling = true;
            playerSneaking = false;

            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            //track positions
            origin = transform.position;
            direction = transform.forward;

            // Handle search types while patrolling
            if(playerSneaking)
            {
                // detect player with line ray
                SphereCastRayPlayerDetection();
            }
            else
            {
                // detect player with radius
                SphereRadiusPlayerDetection();
            }
        }

        public void SphereRadiusPlayerDetection()
        {
            //detect player with distance radius
            Vector3 targetDestination = player.transform.position; // create the target destination based off destination transform
            distanceToPlayer = Vector3.Distance(this.transform.position, targetDestination); //find distance between object and target

            if (distanceToPlayer < sphereDetectionRadius)
            {
                foundPlayer = true; // set the bool for NPC_chase to follow
            }
            else
            {
                foundPlayer = false;
            }
        }

        // function to shoot a single ray and record its data
        public void SphereCastRayPlayerDetection()
        {
            RaycastHit hit;
            if (Physics.SphereCast(origin, rayLineDetectionRadius, direction, out hit, rayDetectionDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                if (hit.transform.gameObject != null)
                {
                    currentHitObject = hit.transform.gameObject; // see what its hit
                    Debug.Log(hit.transform.gameObject.name);
                    currentRayDetectionDistance = hit.distance;

                    if(hit.transform.gameObject.tag == "Player")
                    {
                        foundPlayer = true;
                        
                    }
                    else // hit something else
                    {
                        //set search radius small, so player can hide behind objects
                        rayLineDetectionRadius = 1;
                    }

                }
                else
                {
                    rayLineDetectionRadius = 4; // reset detection bubble
                }
                //NOTE FOUND PLAYER WILL NEVER REVERT TO FALSE
            }
            else // no hit
            {
                currentRayDetectionDistance = rayDetectionDistance;
            }
        }

        //draw relevent gizmos
        private void OnDrawGizmos()
        {
            if(playerSneaking)
            {
                // draw ray line
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, origin + direction * currentRayDetectionDistance);
                Gizmos.DrawWireSphere(origin + direction * currentRayDetectionDistance, rayLineDetectionRadius);
            }
            else
            {
                // draw radius of 'Player NOT sneaking" search
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, sphereDetectionRadius);

            }
        }
    }
}
