using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //Could have type enumerator here to distinguish when object is deleted?
    private float timer;
    public float totalLifeTime = 30f;

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
