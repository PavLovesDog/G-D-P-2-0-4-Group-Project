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

        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform; // transform of player
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject playerViewCamera;

        [Header("Movement Stats")]
        [SerializeField]
        float moveSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float sprintSpeed = 8;
        [SerializeField]
        float walkingSpeed = 2.5f;

        void Start()
        {
            //assignment
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform; // transform of object this script is attached to
            animatorHandler.Initialize();
        }

        public void Update()
        {
            // track smoothing time between frames
            float delta = Time.deltaTime;

            inputHandler.TickInput(delta); // run Tick handle for input listening
            HandleMovement(delta);
            HandleSneaking(delta);
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
                speed = sprintSpeed;
                moveDirection *= speed;
                //inputHandler.moveAmount = 2f;
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

        private void HandleSneaking(float delta)
        {
            if(inputHandler.sneakFlag && inputHandler.moveAmount > 0.1f)
            {
                animatorHandler.animator.SetBool("isSneaking", true);
            }
            else
            {
                animatorHandler.animator.SetBool("isSneaking", false);
            }
        }
    }
}
