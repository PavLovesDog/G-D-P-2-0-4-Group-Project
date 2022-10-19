using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MBF
{
    public class NPCDynamicPatrol : MonoBehaviour
    {
        #region Patrol Variables
        public NavMeshAgent navMeshAgent;
        NPC_Chase npcMovement;
        EnemyManager enemyManager;
        GameManager gameManager;

        [Header("Setup Patrol Variables")]
        public LayerMask layerMask;
        public float waypointSearchRadius;
        public float currentHitDistance;
        [SerializeField]
        public List<GameObject> allWaypoints = new List<GameObject>();
        [SerializeField]
        public List<GameObject> nearbyWaypoints = new List<GameObject>();

        [Header("Destinations")]
        public float waypointDetectionRadius;
        public DynamicWaypoint currentWaypoint;
        public DynamicWaypoint previousWaypoint;

        [Header("Chase Variables")]
        public bool isPersuing;

        [Header("Navigation Variables")]
        [SerializeField]
        public bool patrolWaiting; //dictates whether agent waits on each node
        [SerializeField]
        float waitTime; //total time waited at each node
        bool isTravelling;
        bool isWaiting;
        bool setWaitTime = true;
        public float waitTimer;
        public int waypointsVisited;
        float distanceToDestination;
        Vector3 targetPosition = new Vector3();
        #endregion

        public bool drawWaypointSearchRadius;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            npcMovement = GetComponent<NPC_Chase>();
            enemyManager = GetComponent<EnemyManager>();
            gameManager = FindObjectOfType<GameManager>();

            if (enemyManager.isPatrolling)
                FindFirstWaypoint();
        }


        void Update()
        {
            if (!gameManager.gamePaused)
            {
                // if this enemy is NOT dead, track and patrol
                if (!enemyManager.isDead)
                {
                    if (enemyManager.isPatrolling)
                    {
                        TrackNearbyWaypoints();
                        Patrol();
                    }
                }
            }
        }

        private void Patrol()
        {
            // if NPC hasn't found player, get patrolling!
            if (!enemyManager.foundPlayer)
            {
                navMeshAgent.speed = npcMovement.patrolSpeed;
                isPersuing = false;
                // check if we're close to destination
                if (isTravelling && navMeshAgent.remainingDistance <= 1.0f)
                {
                    isTravelling = false; //turn off bool
                    waypointsVisited++; //track amount of waypoints visited, is this needed???

                    if (patrolWaiting) // if gonna wait. wait
                    {
                        isWaiting = true;
                        waitTimer = 0.0f; // reset wait timer
                    }
                    else
                    {
                        SetDestination();
                    }
                }

                // if waiting at position
                if (isWaiting)
                {
                    //sit still!
                    navMeshAgent.isStopped = true;

                    if (setWaitTime)
                    {
                        setWaitTime = false; // negate bool for one time time setting
                        waitTime = Random.Range(2.0f, 7.0f); // choose random amount of time to wait
                    }
                    waitTimer += Time.deltaTime; // count up timer
                    if (waitTimer >= waitTime) // if its above the amount
                    {
                        isWaiting = false; // stop waitng
                        setWaitTime = true; // set bool for next time set
                        SetDestination(); // move!
                    }
                }
            } // else movement handled by NPC_Chase
            else
            {
                navMeshAgent.isStopped = false; // lets get movng if we're currently chillin'
            }
        }

        private void TrackNearbyWaypoints()
        {
            nearbyWaypoints.Clear();
            //Raycast here and add waypoints to nearbyWaypoints
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, waypointSearchRadius, transform.forward, 1, layerMask, QueryTriggerInteraction.UseGlobal);
            foreach (RaycastHit hit in hits)
            {
                nearbyWaypoints.Add(hit.transform.gameObject); // add gameobject to list
                currentHitDistance = hit.distance;
            }
            distanceToDestination = Vector3.Distance(transform.position, targetPosition); // constantly track distance too
        }

        // Function which propells agent towards its assigned destination
        private void SetDestination()
        {
            if (waypointsVisited > 0) // if its not the first waypoint its visited
            {
                DynamicWaypoint nextWaypoint = currentWaypoint.NextWayPoint(previousWaypoint); // call method to determine next waypoint
                previousWaypoint = currentWaypoint; // set the previous waypoint as the one just visited
                currentWaypoint = nextWaypoint; // update current waypoint to the new waypoint we are moving towards
            }

            // set the vector position and move towards it
            targetPosition = currentWaypoint.transform.position;
            navMeshAgent.SetDestination(targetPosition);
            isTravelling = true;
            navMeshAgent.isStopped = false;
        }

        // Function to be called upon start up to determine where the agent will travel
        // determined on its position in the worrld and proximity to placed waypoints
        private void FindFirstWaypoint()
        {
            //null check
            if (navMeshAgent == null)
            {
                Debug.LogError("the nav mesh component didn't attach to " + gameObject.name);
            }
            else
            {
                //if there is no waypoint, lets find one
                if (currentWaypoint == null)
                {
                    //find all waypoints in scene
                    GameObject[] enemyWaypoints = GameObject.FindGameObjectsWithTag("EnemyWaypoint"); //NOTE Tag may change...
                    foreach (GameObject waypoint in enemyWaypoints)
                    {
                        allWaypoints.Add(waypoint); // add it to list
                    }

                    // if there ARE waypoints, Find the nearest
                    if (allWaypoints.Count > 0)
                    {
                        while (currentWaypoint == null)
                        {
                            //Raycast here and add waypoints to nearbyWaypoints
                            RaycastHit[] hits = Physics.SphereCastAll(transform.position, waypointSearchRadius, transform.forward, 1, layerMask, QueryTriggerInteraction.UseGlobal);
                            foreach (RaycastHit hit in hits)
                            {
                                nearbyWaypoints.Add(hit.transform.gameObject); // add gameobject to list
                            }

                            float shortestDistToNPC = 1000;
                            DynamicWaypoint startWaypoint = gameObject.AddComponent<DynamicWaypoint>();

                            //run through nearby waypoints to find nearest
                            for (int i = 0; i < nearbyWaypoints.Count; i++)
                            {
                                // find distance between NPC and waypoint
                                float distToWaypoint = Vector3.Distance(this.transform.position, nearbyWaypoints[i].transform.position);

                                if (distToWaypoint < shortestDistToNPC) // check if its shorter than previous
                                {
                                    shortestDistToNPC = distToWaypoint; // set new shortest route
                                    startWaypoint = nearbyWaypoints[i].GetComponent<DynamicWaypoint>(); // set the beginning waypoint
                                }
                            }

                            nearbyWaypoints.Clear(); // clear list for later

                            if (startWaypoint != null) // if waypoint was found
                            {
                                currentWaypoint = startWaypoint; // set it as our first destination
                                previousWaypoint = startWaypoint; // also set so errors aren't found on first pass
                            }

                            if (nearbyWaypoints.Count <= 0) // catch for null
                                break;

                        }
                    }
                    else // couldn't find any waypoints!
                    {
                        Debug.LogError("Falied to find any waypoints in scene");
                    }
                }
                //All is well, start up patrol
                SetDestination();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (drawWaypointSearchRadius)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position + transform.forward * currentHitDistance, waypointSearchRadius);
            }
        }
    }
}
