using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MBF
{ 
    public class Companion_AI : MonoBehaviour
    {
        [Header("Tracking Positions")]
        public GameObject player;
        public GameObject position1;
        public GameObject position2;
        public GameObject position3;
        public GameObject sneakPosition;

        InputHandler inputHandler;

        [SerializeField]
        List<Vector3> targetPositions = new List<Vector3>();

        NavMeshAgent navMeshAgent;
        CapsuleCollider thisCollider;
        CapsuleCollider playerCollider;

        //public float distanceToPlayer;
        //public float detectionRadius;
        //public bool foundPlayer;

        public int currentIndex;
        public int previousIndex;
        public float positionTimer;
        public float positionChangeTime;
        bool goingBehindLeft;
        bool goingBehindRight;

        enum position { Right, Behind, Left};

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player");
            inputHandler = FindObjectOfType<InputHandler>();
            thisCollider = GetComponent<CapsuleCollider>();
            playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>();
            //position position; // ?? how to use this agin?

        }

        void Update()
        {
            float delta = Time.deltaTime; // track time
            targetPositions.Clear();
            targetPositions.Add(position2.transform.position); // index 0 // RIGHT of player
            targetPositions.Add(position3.transform.position); // index 1 // BEHIND of player
            targetPositions.Add(position1.transform.position); // index 2 // LEFT of player
            targetPositions.Add(sneakPosition.transform.position); // index 3

            Physics.IgnoreCollision(thisCollider, playerCollider); // always ignore collisions with player

            if (navMeshAgent == null)
            {
                Debug.LogError("Nav Mesh Agent Not attached to " + gameObject.name);
            }
            else
            {
                if(inputHandler.sneakFlag)
                {
                    //if we're sneaking have the companion stay in one location
                    currentIndex = 3;
                }
                else
                {
                    IdleIndexSelector(delta);
                }

                SetDestination(targetPositions[currentIndex]);

                //if(randIndex == 1 && !isBehind)
                //{
                //    if(previousIndex == 0)
                //    {
                //        randIndex++;
                //    }
                //    else if(previousIndex == 2)
                //    {
                //        randIndex--;
                //    }
                //    //isBehind = false;
                //}
                //distanceToPlayer = Vector3.Distance(this.transform.position, targetDestination); //find distance between object and target
                //// check distance to player and adjust bool
                //if (distanceToPlayer < detectionRadius)
                //{
                //    foundPlayer = true;
                //}
                //else
                //{
                //    foundPlayer = false;
                //}

            }
        }

        public void IdleIndexSelector(float delta)
        {
            //randomly choose between 2 positions,
            if (positionTimer >= positionChangeTime)
            {
                previousIndex = currentIndex; // save last index for determinining direction of tyravel
                if (previousIndex == 0) // if we are Right of player
                {
                    currentIndex++;
                    goingBehindLeft = true; // set bool to go position behind player
                    positionTimer = 2.25f; // set timer for short transition
                    positionChangeTime = 3;
                }
                else if (previousIndex == 2) // if companion is Left of player
                {
                    currentIndex--;
                    goingBehindRight = true;
                    positionTimer = 2.25f;
                    positionChangeTime = 3;
                }
                else if (previousIndex == 1) // if companion is Behind of player
                {
                    if (goingBehindLeft)
                    {
                        currentIndex++; // increment in the direction of travel
                        goingBehindLeft = false; // reset bool for next pass
                    }
                    else if (goingBehindRight)
                    {
                        currentIndex--;
                        goingBehindRight = false;
                    }

                    positionTimer = 0; // reset timer completely to stay at new position longer
                    positionChangeTime = Random.Range(3, 8); // randomly choose time to stay in said position
                }
                else // index is probably in sneak psition i.e 3
                {
                    currentIndex = 0;
                }
            }
            else
            {
                positionTimer += delta;
            }
        }

        public void SetDestination(Vector3 targetPosition)
        {

            if (player != null) // if destination exists
            {
                navMeshAgent.SetDestination(targetPosition); // make navemesh agent attach to script move to location
            }
        }
    }
}
