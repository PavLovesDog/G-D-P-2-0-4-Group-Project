using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogLayerControl : MonoBehaviour
{
    ParticleSystem particleSystem;
    Renderer particleRenderer;
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<Renderer>();
            
        //particleSystem.renderer.sortingLayerName = "Foreground";
        particleRenderer.sortingOrder = 0;
        particleRenderer.sortingLayerName = "Foreground";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
