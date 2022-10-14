using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class ThermosPickUp : MonoBehaviour
    {
        PlayerStats playerStats;
        MeshRenderer mesh;
        CapsuleCollider capsuleCollider;
        public float warmthRefillAmount;

        bool refillMeter;

        private void Start()
        {
            playerStats = GameObject.FindObjectOfType<PlayerStats>();
            mesh = GetComponentInChildren<MeshRenderer>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Update()
        {
            // track ability to refil Warmth Meter
            if(refillMeter)
            {
                StartCoroutine(RefillColdMeter());
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                Debug.Log("Picked up a " + gameObject.name);

                //set bool to enable refill
                refillMeter = true;

                //remove mesh and collider on contact
                mesh.enabled = false;
                capsuleCollider.enabled = false;
            }
        }

        IEnumerator RefillColdMeter()
        {
            playerStats.currentColdAmount += warmthRefillAmount * Time.deltaTime; // add warming amount to currentWarmthAmount

            yield return new WaitForSeconds(1); // allow time for it to happen!

            refillMeter = false; // reset bool
            Destroy(gameObject); // destroy object from scene
        }
    }
}
