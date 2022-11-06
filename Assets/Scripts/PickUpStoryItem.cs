using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MBF
{ 
    public class PickUpStoryItem : MonoBehaviour
    {
        public Image StoryImage;
        GameManager gameManager;

        SphereCollider sphereCollider;
        ParticleSystem particles;

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            sphereCollider = GetComponent<SphereCollider>();
            particles = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            HandleParticleEmission();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("Picked up a " + gameObject.name);

                //pause game
                gameManager.gamePaused = true;

                //display window using gameManager functions
                gameManager.EnableImageSprites(gameManager.journalMenu);
                gameManager.EnableText(gameManager.journalMenu);
                gameManager.eventSystem.SetSelectedGameObject(gameManager.journalResumeButton);

                //change Image iwindow
                gameManager.journalImage.sprite = StoryImage.sprite;

                particles.Stop();
                sphereCollider.enabled = false;
            }
        }

        private void HandleParticleEmission()
        {
            //if the object is active within game, make partickles react to pasuing
            if (sphereCollider.enabled == true)
            {
                if (gameManager.gamePaused)
                {
                    if (particles.isPlaying)
                        particles.Stop();
                }
                else
                {
                    if (particles.isStopped)
                    {
                        particles.Play();
                    }
                }
            }
        }
    }
}
