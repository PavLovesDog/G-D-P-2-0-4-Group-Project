using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class AnimatorHandler : MonoBehaviour
    {
        //refernece the animator
        public Animator animator;
        PlayerMovement playerMovement;
        InputHandler inputHandler;
        int vertical;
        int horizontal;

        public bool canRotate;

        public void Initialize()
        {
            animator = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            playerMovement = GetComponentInChildren<PlayerMovement>(); // NO USE NO MORE
            inputHandler = GetComponent<InputHandler>();
        }

        public void UpdateAnimatorValues(float vertMovement, float horiMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0;
            //clamp Vertical values for easy animation change cues
            if (vertMovement > 0 && vertMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (vertMovement > 0.55f)
            {
                v = 1;
            }
            else if (vertMovement < 0 && vertMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (vertMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region hori
            float h = 0;
            //clamp hori values for easy animation change cues
            if (horiMovement > 0 && horiMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horiMovement > 0.55f)
            {
                h = 1;
            }
            else if (horiMovement < 0 && horiMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horiMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            //check for sprinting state and update animator values
            if(isSprinting && vertMovement > 0)
            {
                v = 2;
                h = horiMovement;
            }

            //update parameters within animator
            animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        //function to trigger different animations
        public void PlayTargetAnimation(string targetAnim)
        {
            animator.CrossFade(targetAnim, 0.2f);
        }

        #region Rotation Functions
        //functions to be called to set the roattion bool for animator
        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }
        #endregion

    }
}
