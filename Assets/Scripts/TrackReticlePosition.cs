using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class TrackReticlePosition : MonoBehaviour
    {
        ThrowRock throwRock;
        public float reticleOffset;

        private void Start()
        {
            throwRock = FindObjectOfType<ThrowRock>();
        }
        
        void Update()
        {
            transform.position = new Vector3(transform.position.x, -throwRock.cameraRotationX + reticleOffset, transform.position.z);
        }
    }
}
