using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class Waypoint : MonoBehaviour
    {
        public float debugDrawRadius = 1.0f;

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, debugDrawRadius);
        }
    }
}
