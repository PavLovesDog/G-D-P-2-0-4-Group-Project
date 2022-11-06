using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class SunManager : MonoBehaviour
    {
        GameManager gameManager;
        Transform sunTransform;
        RenderSettings renderSettings;

        Light sunLight;
        [Header("Day Rotation & Speed Variables")]
        public float sunXRotation;
        public float rotationX;
        public float dayLength = 0.01f;
        float sunsetLength = 0.005f;
        public float daySpeed;
        public float daySpeedDivider;

        [Header("Time bools")]
        public bool dayTime;
        bool canChangeToDayTime;
        public bool nightTime;
        bool canChangeToNighttime;

        [Header("Light Variables")]
        public float currentLightAmount;
        public float sunriseSunsetSpeed;


        // Start is called before the first frame update
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            renderSettings = FindObjectOfType<RenderSettings>();

            sunTransform = transform;
            //rotationX = sunTransform.rotation.x;
            sunLight = GetComponent<Light>();
            canChangeToNighttime = true; // set bools for day/night
            dayTime = true;
            canChangeToDayTime = false;
            nightTime = false;
        }

        public void HandleDayStateBools()
        {
            //track daytime/nightime
            if (sunXRotation <= 0 && canChangeToNighttime && dayTime)
            {
                canChangeToNighttime = false;
                dayTime = false;
                nightTime = true;
                canChangeToDayTime = true;

            }
            else if (sunXRotation >= 1 && canChangeToDayTime && nightTime)
            {
                canChangeToDayTime = false;
                nightTime = false;
                dayTime = true;
                canChangeToNighttime = true;
            }
        }

        public void HandleDayNightCycle()
        {
            //NORTE ADJUST DAY SPEED for longer sunsets!
            if (sunXRotation > 0 && sunXRotation < 10)
            {
                //set sunset speed
                daySpeed = sunsetLength;
            }
            else
            {
                if (nightTime)
                {
                    daySpeed = dayLength + 0.005f;
                }
                else
                {
                    daySpeed = dayLength;
                }
            }

            //translate sun!
            transform.Rotate(daySpeed, 0, 0);
        }

        public void HandleLightColour()
        {
            // change colour of light for sunset and sunrise
            if(sunXRotation > 20f)
            {
                sunLight.colorTemperature = 8000;
            }
            else
            {
                sunLight.colorTemperature = sunXRotation * sunriseSunsetSpeed * 10;
            }

            //adjust light intesity for nighttime
            if (sunXRotation < 2.5f)
            {
                //SUNSET
                sunLight.intensity -= Time.deltaTime;
                sunLight.intensity = Mathf.Clamp(sunLight.intensity, 0.01f, 1f);

                //NOTE CHANGE SIHDLSDANFD
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(.75f, 0.25f, Time.deltaTime * 2));
            }
            else
            {
                //SUNRISE
                sunLight.intensity += Time.deltaTime;
                sunLight.intensity = Mathf.Clamp(sunLight.intensity, 0.01f, 1f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!gameManager.gamePaused)
            {
                float delta = Time.deltaTime;
                sunXRotation = WrapAngle(transform.localEulerAngles.x);
                currentLightAmount = sunLight.colorTemperature;

                HandleDayStateBools();
                HandleDayNightCycle();
                HandleLightColour();
            }

            #region Old stuff
            ////track daytime/nightime
            //if (sunXRotation <= 0 && canChangeToNighttime && dayTime)
            //{
            //    canChangeToNighttime = false;
            //    dayTime = false;
            //    nightTime = true;
            //    canChangeToDayTime = true;
            //
            //}
            //else if (sunXRotation >= 1 && canChangeToDayTime && nightTime)
            //{
            //    canChangeToDayTime = false;
            //    nightTime = false;
            //    dayTime = true;
            //    canChangeToNighttime = true;
            //}

            ////rotationX = sunXRotation;
            //rotationX = sunTransform.localRotation.x;
            //rotationX += delta * daySpeed / daySpeedDivider;


            ////calulate rotation of sun!
            //if (dayTime)
            //{
            //    rotationX += delta * daySpeed / daySpeedDivider;
            //}
            //else // nighttime
            //{
            //    rotationX -= delta * daySpeed / daySpeedDivider;
            //}
            //sunTransform.rotation = new Quaternion(rotationX, sunTransform.rotation.y, sunTransform.rotation.z, sunTransform.rotation.w);

            ////NORTE ADJUST DAY SPEED for longer sunsets!
            //if (sunXRotation > 0 && sunXRotation < 10)
            //{
            //    //set sunset speed
            //    daySpeed = sunsetLength;
            //}
            //else
            //{
            //    if (nightTime)
            //    {
            //        daySpeed = dayLength + 0.005f;
            //    }
            //    else
            //    {
            //        daySpeed = dayLength;
            //    }
            //}
            //
            ////translate sun!
            //transform.Rotate(daySpeed, 0, 0);

            //// change colour of light for sunset and sunrise
            //sunLight.colorTemperature = sunXRotation * sunriseSunsetSpeed * 10;
            //
            ////adjust light intesity for nighttime
            //if (sunXRotation < -5f)
            //{
            //    //SUNSET
            //    sunLight.intensity -= Time.deltaTime;
            //    sunLight.intensity = Mathf.Clamp(sunLight.intensity, 0.01f, 1f);
            //}
            //else
            //{
            //    //SUNRISE
            //    sunLight.intensity += Time.deltaTime;
            //    sunLight.intensity = Mathf.Clamp(sunLight.intensity, 0.01f, 1f);
            //}
            #endregion
        }

        //for values between 180-360
        private static float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        //for values between 0-180
        private static float UnWrapAngle(float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }
    }
}
