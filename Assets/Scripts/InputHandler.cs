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
        public bool y_input; // jump button
        public bool left_thumb_click_input;
        public bool Dpad_Right_input; // light torch
        public bool Dpad_Up_Input; // equip torch

        //action bool flags
        public bool sprintFlag;
        public bool sneakFlag;
        bool canSneakSwitch = true;
        public bool jumpFlag;
        public bool equipTorch;
        bool canEquipTorch = true;
        public bool lightTorch;
        bool canLightTorch = true;

        //state bools
        public bool isGrounded = true;
        public bool isInAir = false;

        public PlayerControls playerControls; // reference to input system
        CameraHandler cameraHandler;
        GameManager gameManager;
        
        Vector2 movementInput;
        Vector2 cameraInput;

        private void Start()
        {
            cameraHandler = FindObjectOfType<CameraHandler>(); // acquire camera
            canSneakSwitch = true;
            gameManager = FindObjectOfType<GameManager>(); 
        }

        private void FixedUpdate()
        {
            // track smoothing time between frames
            float delta = Time.deltaTime;

            // Handle camera operations
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                if(!gameManager.gamePaused)
                    cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }

        // Late update function to reset all input bools & flags after each from
        private void LateUpdate()
        {
            left_thumb_click_input = false;
            sprintFlag = false;
            jumpFlag = false;
            equipTorch = false;
            lightTorch = false;
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
            MoveInput();
            HandleSprintInput();
            HandleSneakInput();
            HandleJumpInput();
            HandleTorchInputs();
        }

        private void MoveInput()
        {
            horizontal = movementInput.x; // assign x and y movements to corresponding inputs
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // clamp the absolute values between -1 and 1
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleSprintInput()
        {
            //map the pressing of the button directly to bool
            b_input = playerControls.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

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

        private void HandleSneakInput()
        {
            // map button with input handler
            left_thumb_click_input = playerControls.PlayerActions.Sneak.phase == UnityEngine.InputSystem.InputActionPhase.Performed;


            if(left_thumb_click_input && canSneakSwitch) // listen for click
            {
                canSneakSwitch = false;
                StartCoroutine(sneakFlagSwitch());
            }
        }

        private void HandleJumpInput()
        {
            //map button
            y_input = playerControls.PlayerActions.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            //set bool
            if(y_input)
            {
                jumpFlag = true;
            }
        }

        private void HandleTorchInputs()
        {
            Dpad_Up_Input = playerControls.PlayerActions.EquipTorch.phase == UnityEngine.InputSystem.InputActionPhase.Performed; // equip torch inptu
            Dpad_Right_input = playerControls.PlayerActions.LightTorch.phase == UnityEngine.InputSystem.InputActionPhase.Performed; // light torch inpuit

            if(Dpad_Up_Input && canEquipTorch)
            {
                canEquipTorch = false;
                StartCoroutine(torchEquipTimer());
            }

            if(Dpad_Right_input && canLightTorch)
            {
                canLightTorch = false;
                StartCoroutine(torchLightTimer());
            }
        }

        IEnumerator torchEquipTimer()
        {
            equipTorch = !equipTorch;
            yield return new WaitForSeconds(0.5f);
            canEquipTorch = true;
        }

        IEnumerator torchLightTimer()
        {
            lightTorch = !lightTorch;
            yield return new WaitForSeconds(0.5f);
            canLightTorch = true;
        }

        //Coroutine to handle switching of snealing state, one click at a time
        IEnumerator sneakFlagSwitch()
        {
            sneakFlag = !sneakFlag;
            yield return new WaitForSeconds(0.5f);
            canSneakSwitch = true;
        }
    }
}
