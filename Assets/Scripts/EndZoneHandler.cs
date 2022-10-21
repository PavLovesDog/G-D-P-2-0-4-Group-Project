using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class EndZoneHandler : MonoBehaviour
    {
        public GameObject endZone;
        public GameObject player;

        Vector3 distanceToEnd;
        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Debug.Log("In the end zone");
            }
        }
    }
}
