using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //Could have type enumerator here to distinguish when object is deleted?
    private float timer;
    public float totalLifeTime = 30f;
    public bool doesDelete;

    MeshRenderer[] meshes;

    private void Awake()
    {
        meshes = GetComponents<MeshRenderer>(); // find and add mesh renderes to array
    }

    void Update()
    {
        if (doesDelete)
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

    //Functions for loading meshes when near player
    //Activate all meshrenderes within GameObject
    public void Activate()
    {
        foreach (var mesh in meshes)
            mesh.enabled = true;
    }

    //Dectivate all meshrenderes within GameObject
    public void Deactivate()
    {
        foreach (var mesh in meshes)
            mesh.enabled = false;
    }
}
