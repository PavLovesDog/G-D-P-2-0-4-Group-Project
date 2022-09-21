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

        PlayerControls playerControls; // reference to input system

        Vector2 movementInput;
        Vector2 cameraInput;

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
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x; // assign x and y movements to corresponding inputs
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical)); // clamp the absolute values between -1 and 1
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
