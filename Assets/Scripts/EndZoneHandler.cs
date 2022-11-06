using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MBF
{
    public class EndZoneHandler : MonoBehaviour
    {
        public GameObject endZone;
        public GameObject player;

        GameManager gameManager;

        public float endZoneSize;
        float endZoneRadius;
        float endZonetimer;

        public float playerDistanceToEnd;
        public bool playerAtEndZone;
        public bool loadNextScene;
        public bool reloadScene;

        bool drawEndZoneGizmo;

        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            player = FindObjectOfType<PlayerMovement>().gameObject;
            playerAtEndZone = false;
        }

        void Update()
        {
            //track distance to origin of end
            playerDistanceToEnd = Vector3.Distance(player.transform.position, endZone.transform.position);

            // dynamicallly change size of end zone?
            endZone.transform.localScale = new Vector3(endZoneSize, endZoneSize, endZoneSize);
            endZoneRadius = endZoneSize / 2; // track radius

            // track distance of player to end zone with radius of object accounted for
            if (playerDistanceToEnd - endZoneRadius <= 0)
            {
                playerAtEndZone = true;
            }
            else
            {
                playerAtEndZone = false;
            }

            if(playerAtEndZone)
            {
                // count up a short timer
                endZonetimer += Time.deltaTime;

                // when counter is up
                if(endZonetimer > 2f)
                {
                    //Set bool for GameManager to handle the below
                    gameManager.gameComplete = true;

                    // - pause game
                    // - display the finsih menu screen
                    // - change scene?
                }
            }
            else
            {
                //reset end timer to zero
                endZonetimer = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Debug.Log("In the end zone");
                //playerAtEndZone = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (drawEndZoneGizmo)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, endZoneSize);
            }
        }
    }
}
