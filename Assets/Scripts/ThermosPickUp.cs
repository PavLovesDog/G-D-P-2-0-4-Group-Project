using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class ThermosPickUp : MonoBehaviour
    {
        PlayerStats playerStats;
        public float warmthRefillAmount;

        private void Start()
        {
            playerStats = GameObject.FindObjectOfType<PlayerStats>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit soething/...");

            if(collision.gameObject.tag == "Player")
            {
                Debug.Log("Picked up a " + gameObject.name);

                playerStats.currentColdAmount += warmthRefillAmount;

                //remove form scene
                Destroy(gameObject);
            }
        }
    }
}
