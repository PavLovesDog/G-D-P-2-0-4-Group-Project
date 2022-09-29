using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockVelocity : MonoBehaviour
{
    //Rigidbody rb;
    //public float horiForce = 15;
    //public float vertForce = 4;
    //
    //public Transform cameraDirection;
    //public Transform cameraPivot;
    //
    //
    // destroy variables
    private float timer;
    public float totalLifeTime = 30f;
    //
    //private void Awake()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    cameraDirection = GameObject.FindGameObjectWithTag("MainCamera").transform;
    //    cameraPivot = GameObject.FindGameObjectWithTag("CameraPivot").transform;
    //
    //}
    //
    //void Start()
    //{
    //    //find direction of camera height and adjust force
    //    if(cameraPivot.rotation.x >= -30f) // strongest throw
    //    {
    //        horiForce = 25;
    //        vertForce = 6;
    //    }
    //    else if(cameraPivot.rotation.x >= -20f)
    //    {
    //        horiForce = 20;
    //        vertForce = 5;
    //    }
    //    else if( cameraPivot.rotation.x >= -10)
    //    {
    //        horiForce = 15;
    //        vertForce = 4;
    //    }
    //    else
    //    {
    //        horiForce = 5;
    //        vertForce = 2;
    //    }
    //
    //    //rb.AddForce((cameraDirection.forward * horiForce) + (cameraPivot.up * vertForce), ForceMode.Impulse);
    //}

    // Update is called once per frame
    void Update()
    {
        if(timer >= totalLifeTime)
        {
            Destroy(gameObject); // destroy self
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
