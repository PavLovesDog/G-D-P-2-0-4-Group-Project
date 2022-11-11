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
        GameManager gameManager;
        public NPC_Chase[] nPC_Chase;
        EnemyManager[] enemyManager;
        AudioManager audioManager;
        AudioSource playerAudioSource;
        Torch torch;

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
        bool canPlayJumpSound;
        bool canPlayLandSound;
        float landAudioTimer;

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
        float footstepTimer = 0;

        void Start()
        {
            isDead = false;
            canJumpAnimation = true;
            canPlayJumpSound = true;

            //assignment
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerStats = GetComponent<PlayerStats>();
            gameManager = FindObjectOfType<GameManager>();
            audioManager = FindObjectOfType<AudioManager>();
            playerAudioSource = GetComponent<AudioSource>();

            //find all NPC_Chase scripts
            nPC_Chase = GameObject.FindObjectsOfType<NPC_Chase>();
            
            // find all enemy manager scripts
            enemyManager = GameObject.FindObjectsOfType<EnemyManager>();

            torch = FindObjectOfType<Torch>();

            cameraObject = Camera.main.transform;
            myTransform = transform; // transform of object this script is attached to
            animatorHandler.Initialize();

            fallDetectionRayLength = distanceNeededForFall;
        }

        public void Update()
        {
            if (!gameManager.gamePaused)
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
                    HandleCharacterAudio();
                    HandleTorch();
                }
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
            if(inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
            {
                if (playerStats.isFreezing || inputHandler.sneakFlag)
                {
                    speed = sprintSpeed / 2; // half sprint speed when freezing or sneaking
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

        private void HandleCharacterAudio()
        {
            #region Footsteps
            if (inputHandler.sneakFlag && inputHandler.sprintFlag && inputHandler.moveAmount > 0.1f && inputHandler.isGrounded) // FAST SNEAKING
            {
                HandleFootsteps(0.35f, 0.15f);
            }
            else if (inputHandler.sneakFlag && inputHandler.moveAmount > 0.1f && inputHandler.isGrounded) // SLOW SNEAKING
            {
                HandleFootsteps(0.45f, 0.15f);
            }
            else if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f && inputHandler.isGrounded) // SPRINTING
            {
                HandleFootsteps(0.25f, 0.25f);
            }
            else if (inputHandler.moveAmount > 0.1f && inputHandler.moveAmount <= 1f && inputHandler.isGrounded) // WALKING
            {
                HandleFootsteps(0.38f, 0.25f);
            }
            #endregion

            #region Jump and Land
            if (inputHandler.jumpFlag && canPlayJumpSound)
            {
                canPlayJumpSound = false;
                StartCoroutine(JumpSoundDelay());
            }

            //set bool and increment timer whilst in air
            if(inputHandler.isInAir)
            {
                //set some bool true
                canPlayLandSound = true;
                landAudioTimer += Time.deltaTime;
            }

            //if has been in air long enough, play the landing sound on impact
            if(canPlayLandSound && inputHandler.isGrounded && landAudioTimer > 1f) // moment when 
            {
                canPlayLandSound = false;
                landAudioTimer = 0;
                //play audio landing sound
                audioManager.PlayAudio(playerAudioSource, audioManager.land, 0.3f);
            }
            #endregion
        }

        private void HandleFootsteps(float delay, float volume)
        {
            //play footstep audio
            footstepTimer += Time.deltaTime;
            if (footstepTimer > delay)
            {
                audioManager.PlayAudio(playerAudioSource, audioManager.footsteps, volume);
                footstepTimer = 0;
            }
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

        private void HandleTorch()
        {
            //EQUIP
            if(inputHandler.equipTorch && torch.isEquipped == false) // if not holding the torch
            {
                // equpid da torche
                torch.Equip();
            }
            else if(inputHandler.equipTorch && torch.isEquipped == true) // if torch is already equipped
            {
                //unneqiup da tawrch
                torch.Unequip();
            }

            //LIGHT
            if(inputHandler.lightTorch && torch.isEquipped == true && torch.isLit == false)
            {
                torch.LightTorch();
            }
            else if(inputHandler.lightTorch && torch.isEquipped == true && torch.isLit == true)
            {
                torch.ExtinguishTorch();
            }
        }

        IEnumerator PlayJumpAnimation(float jumpTime)
        {
            animatorHandler.PlayTargetAnimation("Jump");
            yield return new WaitForSeconds(jumpTime);
            canJumpAnimation = true;
            canPlayJumpSound = true;
        }

        IEnumerator JumpSoundDelay()
        {
            audioManager.PlayAudio(playerAudioSource, audioManager.jump, 0.25f);
            yield return new WaitForSeconds(1f);
            canPlayJumpSound = true;
        }
    }
}
