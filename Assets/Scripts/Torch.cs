using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class Torch : MonoBehaviour
    {
        //array to hold all found mesh renderers in attached GameObject
        [SerializeField]
        MeshRenderer[] meshes;
        [SerializeField]
        Light lightsoure;
        [SerializeField]
        ParticleSystem particle;

        public bool isEquipped;
        public bool isLit;

        private void Awake()
        {
            // find and add mesh renderes to array
            meshes = GetComponentsInChildren<MeshRenderer>();
            lightsoure = GetComponentInChildren<Light>();
            particle = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            Unequip();
            isEquipped = false;
            isLit = false;
        }

        public void LightTorch()
        {
            lightsoure.enabled = true;
            particle.Play();
            isLit = true;
        }

        public void ExtinguishTorch()
        {
            lightsoure.enabled = false;
            particle.Stop();
            isLit = false;
        }

        public void Equip()
        {
            isEquipped = true;

            //Activate all meshrenderes within GameObject
            foreach (var mesh in meshes)
                mesh.enabled = true;
        }

        public void Unequip()
        {
            isEquipped = false;
            isLit = false;

            //Deactivate all meshrenderes within GameObject
            foreach (var mesh in meshes)
                mesh.enabled = false;

            //Turn Off light
            lightsoure.enabled = false;

            //Stop Fire Emissions
            particle.Stop();
        }
    }
}
