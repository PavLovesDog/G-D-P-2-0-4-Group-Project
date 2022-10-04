using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class DynamicWaypoint : Waypoint
    {
        [SerializeField]
        float connectivityRadius = 30f;
    
        public List<DynamicWaypoint> connections;
    
        void Start()
        {
            //find all the enemy waypoints in scene
            GameObject[] waypoints = GameObject.FindGameObjectsWithTag("EnemyWaypoint");
    
            //set up list
            connections = new List<DynamicWaypoint>();

            // run through found enemy waypoints
            for(int i = 0; i < waypoints.Length; i++) //This can be a FOREACH loop
            {
                DynamicWaypoint nextWaypoint = waypoints[i].GetComponent<DynamicWaypoint>(); // assign the next waypoint to travel to
                if(nextWaypoint != null)
                {
                    // if next waypoint is within range AND is not the current waypoint
                    if(Vector3.Distance(transform.position, nextWaypoint.transform.position) <= connectivityRadius && nextWaypoint != this)
                    {
                        connections.Add(nextWaypoint);
                    }
                }
            }
        }
    
        //Set up visible gizmos in inspector
        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);
    
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, connectivityRadius);
        }
    
        //method to determine next waypoint to be selected
        public DynamicWaypoint NextWayPoint(DynamicWaypoint previousWaypoint)
        {
            if(connections.Count == 0)
            {
                //if no waypoints, log error
                Debug.LogError("No waypoints found.");
                return null;
            }
            else if (connections.Count == 1 && connections.Contains(previousWaypoint))
            {
                //only one waypoint in list, and its the last, then go back
                return previousWaypoint;
            }
            else // find other waypoint, thats no previous
            {
                DynamicWaypoint nextWaypoint;
                int nextIndex = 0;
    
                // loop through and find next destination, while next is the same as previous destination
                do
                {
                    nextIndex = Random.Range(0, connections.Count);
                    nextWaypoint = connections[nextIndex];
    
                } while (nextWaypoint == previousWaypoint); // this condition means it will ALWAYS move forward ...
    
                return nextWaypoint;
            }
        }
    }
}
