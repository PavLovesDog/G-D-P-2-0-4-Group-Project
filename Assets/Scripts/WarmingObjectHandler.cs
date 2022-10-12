using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class WarmingObjectHandler : MonoBehaviour
    {
        public PlayerStats playerStats;
        public GameObject player;

        [SerializeField]
        List<GameObject> playerObject = new List<GameObject>();
        public LayerMask layerMask;

        public Vector3 origin;
        private Vector3 direction;
        float currentHitDistance;
        public float warmthRadius;
        public float distanceToPlayer;

        private void Start()
        {
            // get reference to player in world
            player = GameObject.FindGameObjectWithTag("Player");
            playerStats = FindObjectOfType<PlayerStats>();
        }

        private void Update()
        {
            //track position
            origin = transform.position;
            direction = transform.forward;

            //Track distance to player?
            distanceToPlayer = Vector3.Distance(origin, player.transform.position);

            //if(distanceToPlayer < warmthRadius)
            //{
            //    playerStats.isWarming = true; // set bool for warmth replpenishment
            //    Debug.Log("Player is close enough to feel the warmth...");
            //
            //}
            //else
            //{
            //    playerStats.isWarming = false;
            //}

            DetectPlayerSphereRayCast();
        }

        public void DetectPlayerSphereRayCast()
        {
      
            currentHitDistance = 0;
            playerObject.Clear(); // clear list becfore cast
            RaycastHit[] hits = Physics.SphereCastAll(origin, warmthRadius, direction, 0, layerMask, QueryTriggerInteraction.UseGlobal);

            foreach (RaycastHit hit in hits)
            {
                playerObject.Add(hit.transform.gameObject);
                currentHitDistance = hit.distance;
            }

            // WITHIN SEARCH RADIUS
            if (playerObject.Count >= 1)
            {
                playerStats.isWarming = true;
    
            }
        }

        //function to draw gizmos!
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Debug.DrawLine(origin, origin + direction * currentHitDistance);
            Gizmos.DrawWireSphere(origin + direction * currentHitDistance, warmthRadius);
        }
    }
}
