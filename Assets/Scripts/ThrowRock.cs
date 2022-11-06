using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class ThrowRock : MonoBehaviour
    {
        InputHandler inputHandler;
        AudioManager audioManager;

        public float damage = 10f;
        //public float range = 50f;
        //public float impactForce = 30f;
        public float cameraRotationX;
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

        private void Start()
        {
            //currentAmmo = maxAmmo; // start with full rocks?
            inputHandler = GetComponent<InputHandler>();
            audioManager = FindObjectOfType<AudioManager>();
            cameraDirection = GameObject.FindGameObjectWithTag("MainCamera").transform;
            cameraPivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
            canThrow = true;
        }

        // Update is called once per frame
        void Update()
        {
            //track camera x rotation and map it to throwing arc
            cameraRotationX = cameraPivot.localRotation.x * 100;
            arc = -cameraPivot.localRotation.x * 100;
            //arc = -cameraPivot.localRotation.x * arcForce;
            //arc = cameraPivot.localEulerAngles.x / 100;
            //arc = Mathf.Clamp(arc, -35, 35);

            HandleRockThrow(arc);
        }

        public void HandleRockThrow(float arc)
        {
            // listen to input controls for button click
            left_click_input = inputHandler.playerControls.PlayerActions.Throw.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

            if (left_click_input && canThrow) // if button has been clicked
            {

                canThrow = false;
                if (currentAmmo > 0)
                {
                    currentAmmo--;

                    //spawn rock
                    StartCoroutine(SpawnRockProjectile(arc));
                }  
                else // no ammo
                {
                    canThrow = true;
                }
            }
        }

        IEnumerator SpawnRockProjectile(float arc)
        {
            //play audio sound in this moment
            audioManager.PlayAudio(audioManager.audioSource, audioManager.throwItem, 0.65f);

            GameObject projectile = Instantiate(rock) as GameObject;
            projectile.transform.position = rockSpawnPoint.position + cameraPivot.forward;
            projectile.transform.rotation = cameraDirection.rotation;// cameraPivot.rotation; 
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = cameraDirection.forward * throwForce + cameraPivot.up * (arc/2);

            yield return new WaitForSeconds(reloadTime); // pause before it can happen agian
            canThrow = true;
        }

        //upon the player (because thats the game object this script is attached to) triggering the 'other' colllider
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Projectile")
            {
                if (currentAmmo < maxAmmo)
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
}
