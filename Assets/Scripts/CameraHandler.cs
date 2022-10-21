using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform; // transform camera will go to
        public Transform cameraTransform; // trabsform of ACTUAL camera
        public Transform cameraPivotTransform; // how the camera swivels
        private Transform myTransform;
        private Vector3 cameraTransformPosition; // position of camera
        private LayerMask ignoreLayers; // used for camera collision
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;
        GameManager gameManager;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        public float cameraTransitionSpeed = 2;
        float cameraRotationChangeSpeed = 2;
        public float offsetXAmount = 0.35f;
        public float offsetYAmount;

        private Vector3 currentCameraOffset;
        private Vector3 previousCameraOffset;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        public Vector3 currentCamPostion = new Vector3();
        public Vector3 previousCamPosition = new Vector3();
        private float startTime;

        private void Awake()
        {
            singleton = this;
            myTransform = transform; // myTransfomr is equal to transform of this game object
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10); // ignore layers R
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Application.targetFrameRate = 60; // set target framrate for forced smoothness
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            startTime = Time.time;
        }

        //Follow target transform (i.e the player)
        public void FollowTarget(float delta)
        {
            // target speed equals a linear interpolation between cameras current transform & target transform
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, 
                                                        targetTransform.position, 
                                                        ref cameraFollowVelocity, 
                                                        delta / followSpeed);

            //Handle where the camera sits while game is paused or not, camera drops with player sitting while paused, raises when player resumes
            if(!gameManager.gamePaused)
            {
                currentCameraOffset = new Vector3(cameraTransform.localPosition.x + offsetXAmount, 0.0f, 0.0f);
                previousCameraOffset = new Vector3(cameraTransform.localPosition.x + offsetXAmount, cameraTransform.localPosition.y + offsetYAmount, 0.0f);

                currentCamPostion = targetPosition + currentCameraOffset;
                previousCamPosition = targetPosition + previousCameraOffset;
            }
            else // IS PAUSED
            {
                currentCameraOffset = new Vector3(cameraTransform.localPosition.x + offsetXAmount, cameraTransform.localPosition.y + offsetYAmount, 0.0f);
                previousCameraOffset = new Vector3(cameraTransform.localPosition.x + offsetXAmount, 0.0f, 0.0f);

                currentCamPostion = targetPosition + currentCameraOffset;
                previousCamPosition = targetPosition + previousCameraOffset;
            }

            float transition = (Time.time - startTime) / cameraTransitionSpeed;
            myTransform.position = Vector3.Slerp(previousCamPosition, currentCamPostion, transition);

            //myTransform.position = targetPosition + currentCameraOffset;

            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot); // clamp camera pivot

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);

            if(gameManager.pauseCameraTransition) // if we want the camera to slerp from pause position (Handled by pause)
            {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, cameraRotationChangeSpeed * delta);
            }
            else // else in play mode
            {
                myTransform.rotation = targetRotation; 
            }

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);

            cameraPivotTransform.localRotation = targetRotation;
            //myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, cameraStartSpeed * delta);

        }

        private void HandleCameraCollisions(float delta)
        {
            // set up raycats and direction to cast ray
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            // check if cast collides with something within the distance
            if(Physics.SphereCast(cameraPivotTransform.position, 
                                  cameraSphereRadius, 
                                  direction, 
                                  out hit, 
                                  Mathf.Abs(targetPosition), 
                                  ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point); // find distance of hit
                targetPosition = -(dis - cameraCollisionOffset); // set new camera position with offset
            }

            // check
            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            // lerp between new found target position and old position
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
