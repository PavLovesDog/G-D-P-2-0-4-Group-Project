using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class ThrowRock : MonoBehaviour
    {
        InputHandler inputHandler;

        public float damage = 10f;
        //public float range = 50f;
        //public float impactForce = 30f;
        public float throwForce;
        public float arc;
        public float arcForce;

        public int maxAmmo = 15;
        public int currentAmmo;
        public float reloadTime = 1f;

        public bool left_click_input;
        public bool canThrow;
        public Transform rockSpawnPoint;
        public Transform cameraDirection;
        public Transform cameraPivot;
        public GameObject rock;
        public GameObject rockParent;

        private void Start()
        {
            currentAmmo = maxAmmo; // start with full rocks?
            inputHandler = GetComponent<InputHandler>();
            cameraDirection = GameObject.FindGameObjectWithTag("MainCamera").transform;
            cameraPivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
            canThrow = true;
        }

        // Update is called once per frame
        void Update()
        {
            arc = -cameraPivot.localRotation.x * arcForce;

            HandleRockThrow(arc);
        }

        public void HandleRockThrow(float arc)
        {
            // listen to input controls for button click
            left_click_input = inputHandler.playerControls.PlayerActions.Throw.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            if (left_click_input && canThrow) // if button has been clicked
            {
                canThrow = false;
                if (currentAmmo > 0)
                {
                    currentAmmo--;

                    //spawn rock
                    StartCoroutine(SpawnRockProjectile(arc));
                }
            }
        }

        IEnumerator SpawnRockProjectile(float arc)
        {
            GameObject projectile = Instantiate(rock) as GameObject;
            projectile.transform.position = rockSpawnPoint.position + cameraPivot.forward;
            projectile.transform.rotation = cameraDirection.rotation;// cameraPivot.rotation; 
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = cameraDirection.forward * throwForce + cameraPivot.up * arc;

            yield return new WaitForSeconds(reloadTime); // pause before it can happen agian
            canThrow = true;
        }

        //upon the player (because thats the game object this script is attached to) triggering the 'other' colllider
        private void OnTriggerEnter(Collider other)
        {
            if(currentAmmo < maxAmmo)
            {
                currentAmmo++;
                Debug.Log("You picked up a " + other.gameObject.name);
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("No room for more " + other.gameObject.name + "s...");
            }
        }
    }
}
