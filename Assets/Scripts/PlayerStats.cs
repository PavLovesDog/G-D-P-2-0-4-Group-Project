using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health Variables")]
        public float currentHealth;
        public float maxHealth;
        bool deathCheck;

        [Header("Environment Variables")]
        public float coldIntensity;
        public float currentColdAmount;
        public float maxColdAmount;
        public bool isFreezing;
        public bool isWarming;
        public float warmthReplenishRate;

        //public bool refillMeter;

        public HealthBar healthBar;
        public ColdMeter coldMeter;
        public PlayerMovement playerMovement;
        AnimatorHandler animatorHandler;
        // Reference to animator handler to play damage and death animations??

        private void Awake()
        {
            healthBar = FindObjectOfType<HealthBar>();
            coldMeter = FindObjectOfType<ColdMeter>();
            playerMovement = GetComponent<PlayerMovement>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }


        void Start()
        {
            //Set health variables
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrenthealth(currentHealth);
            deathCheck = true;

            //set cold meter variables
            currentColdAmount = maxColdAmount;
            coldMeter.SetMaxCold(maxColdAmount);
            coldMeter.SetCurrentCold(currentColdAmount);
            isWarming = false;
        }

        void Update()
        {
            HandleWarmingAndFreezingStates();

            //visuall track changes
            coldMeter.SetCurrentCold(currentColdAmount);
            healthBar.SetCurrenthealth(currentHealth);

            //death check
            {
                if (currentHealth <= 0) // Death condition
                {
                    currentHealth = 0;
                    //Play death animation
                    if(deathCheck)
                    {
                        deathCheck = false;
                        animatorHandler.PlayTargetAnimation("Death");
                    }
                    //stop move functions
                    playerMovement.isDead = true;
                }
            }
        }

        private void HandleWarmingAndFreezingStates()
        {
            //Check if near a warmth source
            if (!isWarming) // if NOT near warm
            {
                //reduce colde meter
                currentColdAmount -= coldIntensity * Time.deltaTime;
                currentColdAmount = Mathf.Clamp(currentColdAmount, 0, 100);
            }
            else
            {
                //Warm us up!
                currentColdAmount += warmthReplenishRate * Time.deltaTime;
                currentColdAmount = Mathf.Clamp(currentColdAmount, 0, 100);
                isWarming = false;
            }

            //check if too cold!
            if (currentColdAmount <= 0)
            {
                //player is freezing! 
                isFreezing = true;
                //slow movement,
                playerMovement.moveSpeed = 2.5f;
                //deplete health!
                currentHealth -= coldIntensity * Time.deltaTime;
            }
            else
            {
                isFreezing = false;
                playerMovement.moveSpeed = 5f;
            }
        }

        public void DoDamage(int damage)
        {
            currentHealth -= damage; // minus damage
            healthBar.SetCurrenthealth(currentHealth); // adjust slider

            if(currentHealth <= 0) // Death condition
            {
                currentHealth = 0;
                //Play death animation
                animatorHandler.PlayTargetAnimation("Death");
                //stop move functions
                playerMovement.isDead = true;
            }
        }

        //WHICH SCRIPT SHOULD HANDLE COLLISION DAMAGE>>??
        // Handle collision with enemies
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.tag == "Enemy") // touched an enemy!
        //    {
        //        DoDamage(10);
        //    }
        //}
    }
}
