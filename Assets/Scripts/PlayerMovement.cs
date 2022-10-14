/* 
 * The structure of the AnimationHandler, PlayerMovement & InputHandler scripts were derived 
 * from a Youtube tutorial series called "creat DARK SOULS in Unity" by Sebastian Graves. 
 * Inspiriation for how things are handled and they way these scripts talk to each other lend
 * to this design but have been modified heavily  by myself, Charles Bird, to suit the needs of this 
 * project and functionality.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class PlayerMovement : MonoBehaviour
    {
        // references
        Transform cameraObject;
        InputHandler inputHandler;
        PlayerStats playerStats;
        public NPC_Chase[] nPC_Chase;
        EnemyManager[] enemyManager;

        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // transform of player
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject playerViewCamera;

        [Header("Movement Stats")]
        [SerializeField]
        public float moveSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float sprintSpeed = 8;
        [SerializeField]
        float walkingSpeed = 2.5f;
        [SerializeField]
        float jumpForce = 10;
        public float jumpTimer = 0;
        public bool canJump = true;
        public bool isDead;
        bool canJumpAnimation;

        //This list BETTER suited for NPC...
        //[Header("Raycast variables")]
        //[SerializeField]
        //float detectionRadius;
        //[SerializeField]
        //float detectionDistance;
        //[SerializeField]
        //Vector3 lookDirection = new Vector3();
        //[SerializeField]
        //public GameObject currentHitObject;
        //[SerializeField]
        //LayerMask layerMask;
        //private float currentHitDistance;

        //This list BETTER suited for NPC...
        [Header("Falling & Raycast variables")]
        [SerializeField]
        float initialFallSpeed;
        [SerializeField]
        float fallVelocity;
        [SerializeField]
        float gravityIntensity;
        [SerializeField]
        float distanceNeededForFall;
        float fallDetectionRayLength;
        [SerializeField]
        Vector3 rayDirection = new Vector3();
        [SerializeField]
        public GameObject currentHitObject;
        [SerializeField]
        LayerMask layerMask; // NOt needed now??
        float inAirTimer = 0f;

        void Start()
        {
            isDead = false;
            canJumpAnimation = true;

            //assignment
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerStats = GetComponent<PlayerStats>();

            //find all NPC_Chase scripts
            nPC_Chase = GameObject.FindObjectsOfType<NPC_Chase>();
            
            // find all enemy manager scripts
            enemyManager = GameObject.FindObjectsOfType<EnemyManager>();

            cameraObject = Camera.main.transform;
            myTransform = transform; // transform of object this script is attached to
            animatorHandler.Initialize();

            fallDetectionRayLength = distanceNeededForFall;
        }

        public void Update()
        {
            // track smoothing time between frames
            float delta = Time.deltaTime;
            if (!isDead)
            {
                inputHandler.TickInput(delta); // run Tick handle for input listening
                HandleMovement(delta);
                HandleSneaking();
                HandleFalling(delta);
                HandleJumping(delta);
            }
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleMovement(float delta)
        {
            // handle Rotations, based on animator
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }

            //Handle animations from blend tree
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, inputHandler.b_input);

            // get facing/move direction from camera direciton
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0; // Locked in to position NOT GOOD

            // set new found direction multtplied by set speed
            float speed = moveSpeed;

            // check for sprinting
            if(inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f && !inputHandler.sneakFlag)
            {
                if(playerStats.isFreezing)
                {
                    speed = sprintSpeed / 2;
                }
                else
                {
                    speed = sprintSpeed;
                }
                moveDirection *= speed;
            }
            else // not sprinting
            {
                if (inputHandler.moveAmount < 0.5) // check for walking
                {
                    moveDirection *= walkingSpeed;
                }
                else if (inputHandler.sneakFlag && inputHandler.moveAmount > 0.1f)
                {
                    moveDirection *= walkingSpeed;
                }
                else // reset speed as regular pace
                {
                    moveDirection *= speed;
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity; // set movements to act upon rigid body
        }

        //Handle the player models rotation
        private void HandleRotation(float delta)
        {
            Vector3 targetDirection = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            // Set look directions based on input
            targetDirection = cameraObject.forward * inputHandler.vertical;
            targetDirection += cameraObject.right * inputHandler.horizontal;

            targetDirection.Normalize();
            targetDirection.y = 0; // ensure no y-axis rotation will be happening

            if (targetDirection == Vector3.zero)
                targetDirection = myTransform.forward;

            float rs = rotationSpeed;

            //apply rotation
            Quaternion lookTargetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, lookTargetRotation, rs * delta);

            myTransform.rotation = targetRotation;
        }
        #endregion

        private void HandleSneaking()
        {
            if (inputHandler.sneakFlag)
            {
                animatorHandler.animator.SetBool("isSneaking", true);

                //// reduce detection distance of enemies
                //foreach (NPC_Chase script in nPC_Chase)
                //{
                //    // half detection radius of each enemy
                //    script.currentDetectionRadius = script.detectionRadius / 3;
                //    //NOTE above could be handled in NPC script. if the player isSNeaking, they switch to raycast detections?
                //    //So they just see whats in front of them??
                //}

                // run through every instance of enemy and set enemy manager bool
                foreach (EnemyManager script in enemyManager)
                {
                    script.playerSneaking = true;
                }
            }
            else // not snekaing
            {
                animatorHandler.animator.SetBool("isSneaking", false);
                //foreach (NPC_Chase script in nPC_Chase)
                //{
                //    // half detection radius of each enemy
                //    script.currentDetectionRadius = script.detectionRadius;
                //}

                // run through every instance of enemy and set enemy manager bool
                foreach (EnemyManager script in enemyManager)
                {
                    script.playerSneaking = false;
                }
            }
        }

        private void HandleFalling(float delta)
        {
            // update animator values for animation
            animatorHandler.animator.SetBool("isInAir", inputHandler.isInAir);
            animatorHandler.animator.SetBool("isGrounded", inputHandler.isGrounded);

            #region Cast Downwards Ray
            rayDirection = -Vector3.up; // shoot ray down
            // draw ray for debugging
            Debug.DrawRay(transform.position + Vector3.up, rayDirection * fallDetectionRayLength, Color.red, 0.1f, false);
            RaycastHit hit;
            // cast ray downwards from centre of character to check for collisions
            if (Physics.Raycast(transform.position + Vector3.up, rayDirection, out hit, fallDetectionRayLength))
            {
                if (hit.transform.gameObject != null)
                {
                    currentHitObject = hit.transform.gameObject;

                    //change air time bools
                    inputHandler.isGrounded = true;
                    inputHandler.isInAir = false;
                }

            }
            else // no hit
            {
                currentHitObject = null;

                //change air time bools
                inputHandler.isGrounded = false;
                inputHandler.isInAir = true;
            }
            #endregion

            //handle fall velocity
            if(inputHandler.isInAir)
            {
                //increment timer for fall animation, fall anim only plays after 0.25f seconds has passed. this is set in the animator
                inAirTimer += Time.deltaTime;
                animatorHandler.animator.SetFloat("inAirTimer", inAirTimer);

                //calculate fall speed based on time in air and gravity
                fallVelocity += delta * (gravityIntensity);
                rigidbody.AddForce((rayDirection * fallVelocity * 1000) + moveDirection); // add falling speed to rigidbody
                rigidbody.AddForce(moveDirection.normalized * 4f, ForceMode.Impulse); // give a little nudge of the edge in direct heading, to avoid getting stuck on ledge

            }
            else
            {
                fallVelocity = initialFallSpeed;
                inAirTimer = 0f;
            }
        }

        private void HandleJumping(float delta)
        {
            if(inputHandler.jumpFlag)
            {
                 // set timer to handle jumps
                // play animation
                jumpTimer = 4f;
                if(canJumpAnimation)
                {
                    canJumpAnimation = false;
                    StartCoroutine(PlayJumpAnimation(jumpTimer));
                }
                
            }

            //whilst the timer is running, compute the jump
            if (jumpTimer > 0)
            {
                jumpTimer -= delta; // count down timer

                if (inputHandler.isGrounded) // if player lands, stop calculating jump
                    jumpTimer = 0;

                fallDetectionRayLength = 0; // trip raycast to be in air

                if (inputHandler.isInAir)
                {
                    fallDetectionRayLength = distanceNeededForFall; // reset ray cast length to detect ground
                    float jump = 0;
                    jump += delta * jumpForce * 100;
                    rigidbody.AddForce(Vector3.up * jump * 1000);
                }
            }
            else
            {
                fallDetectionRayLength = distanceNeededForFall;// reset
                jumpTimer = 0;
            }
        }

        IEnumerator PlayJumpAnimation(float jumpTime)
        {
            animatorHandler.PlayTargetAnimation("Jump");
            yield return new WaitForSeconds(jumpTime);
            canJumpAnimation = true;
        }


        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.tag == "Enemy") // touched an enemy!
        //    {
        //        playerStats.TakeDamage(10);
        //    }
        //}
        //// may not be needed for single ray shot here...
        //public void SphereCastRay(Vector3 direction)
        //{
        //    RaycastHit hit;
        //    if (Physics.SphereCast(transform.position, detectionRadius, lookDirection, out hit, detectionDistance, layerMask, QueryTriggerInteraction.UseGlobal))
        //    {
        //        if (hit.transform.gameObject != null)
        //        {
        //            currentHitObject = hit.transform.gameObject;
        //            Debug.Log(hit.transform.gameObject.name);
        //            currentHitDistance = hit.distance;
        //        }
        //
        //    }
        //    else // no hit
        //    {
        //        currentHitDistance = detectionDistance;
        //        currentHitObject = null;
        //    }
        //}
    }
}
