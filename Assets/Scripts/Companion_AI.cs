using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace navemesh_Tests
{ 
    public class Companion_AI : MonoBehaviour
    {
        [SerializeField]
        public GameObject player;
        public GameObject position1;
        public GameObject position2;
        public GameObject position3;

        [SerializeField]
        List<Vector3> targetPositions = new List<Vector3>();

        NavMeshAgent navMeshAgent;

        public float distanceToPlayer;
        public float detectionRadius;
        public bool foundPlayer;

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
            position position; // ?? how to use this agin?

        }

        void Update()
        {
            float delta = Time.deltaTime; // track time
            targetPositions.Clear();
            targetPositions.Add(position2.transform.position); // index 0 // RIGHT of player
            targetPositions.Add(position3.transform.position); // index 1 // BEHIND of player
            targetPositions.Add(position1.transform.position); // index 2 // LEFT of player

            if (navMeshAgent == null)
            {
                Debug.LogError("Nav Mesh Agent Not attached to " + gameObject.name);
            }
            else
            {
                IdleIndexSelector(delta);

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
                    positionChangeTime = Random.Range(3, 6); // randomly choose time to stay in said position
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
