using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class High_Poly_Model : MonoBehaviour
    {
        MeshRenderer[] meshes;

        private void Awake()
        {
            meshes = GetComponentsInChildren<MeshRenderer>();
        }

        public void Activate()
        {
            foreach (var mesh in meshes)
                mesh.enabled = true;
        }

        public void Deactivate()
        {
            foreach (var mesh in meshes)
                mesh.enabled = false;
        }
    }
}
