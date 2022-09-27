using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Movement code derived from Sebastian graves Youtube series: Dark Souls in Unity.
 * This series taught me about the input system and how to utilise it 
*/


namespace MBF
{
    public class InputHandler : MonoBehaviour
    {
        //move variables
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        //button inputs
        public bool b_input; // Sprinting input
        public bool left_thumb_click_input;

        //action bool flags
        public bool sprintFlag;
        public bool sneakFlag;

        PlayerControls playerControls; // reference to input system
        CameraHandler cameraHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Start()
        {
            cameraHandler = FindObjectOfType<CameraHandler>(); // acquire camera
        }

        private void FixedUpdate()
        {
            // track smoothing time between frames
            float delta = Time.deltaTime;

            // Handle camera operations
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        // Late update function to reset all input bools & flags after each from
        private void LateUpdate()
        {
            left_thumb_click_input = false;

            sprintFlag = false;
        }

        public void OnEnable()
        {
            if(playerControls == null)
            {
                //create them!
                playerControls = new PlayerControls();
                //assign inputs from manager to movement vector
                playerControls.PlayerMovement.Movement.performed += playerControls => movementInput = playerControls.ReadValue<Vector2>();
                //assign inputs from manager to camera vector
                playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }

            playerControls.Enable(); // begin listening for inputs
        }

        private void OnDisable()
        {
            playerControls.Disable(); // stop listening for input
        }

        //Function which handles input specific calls, and unifies them
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleSprintInput(delta);
            HandleSneakInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x; // assign x and y movements to corresponding inputs
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // clamp the absolute values between -1 and 1
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleSprintInput(float delta)
        {
            //map the pressing of the button directly to bool
            b_input = playerControls.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            // listen for button press & assign bool flag
            if(b_input)
            {
                sprintFlag = true;
            }
            else
            {
                sprintFlag = false;
            }
        }

        private void HandleSneakInput(float delta)
        {
            // map button with input handler
            left_thumb_click_input = playerControls.PlayerActions.Sneak.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            if(left_thumb_click_input) // listen for click
            {
                sneakFlag = true;
            }
            else
            {
                sneakFlag = false;
            }
        }
    }
}
