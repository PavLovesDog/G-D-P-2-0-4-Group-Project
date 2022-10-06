using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    Transform sunTransform;
    Light sunLight;
    public float sunXRotation;
    public float daySpeed;
    public float daySpeedDivider;

    public float currentLightAmount;
    public float sunriseSunsetSpeed;

    // Start is called before the first frame update
    void Start()
    {
        sunTransform = transform;
        sunLight= GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        float rotationX = sunTransform.rotation.x;
        sunXRotation = WrapAngle(transform.localEulerAngles.x);
        currentLightAmount = sunLight.colorTemperature;
        

        //calulate rotation of sun!
        rotationX += delta * daySpeed / daySpeedDivider;
        sunTransform.rotation = new Quaternion(rotationX, sunTransform.rotation.y, sunTransform.rotation.z, sunTransform.rotation.w);
        
        // change colour of light for sunset and sunrise
        sunLight.colorTemperature = sunXRotation * sunriseSunsetSpeed * 10;

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
