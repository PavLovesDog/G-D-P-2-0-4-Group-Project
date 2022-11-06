using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    private float timer;
    public float totalLifeTime = 1f;

    void Update()
    {
        if (timer >= totalLifeTime)
        {
            Destroy(gameObject); // destroy self
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
