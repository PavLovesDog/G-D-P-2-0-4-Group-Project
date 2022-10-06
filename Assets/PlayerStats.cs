using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class PlayerStats : MonoBehaviour
    {
        public int currentHealth;
        public int maxHealth;

        public float coldIntensity;
        public float currentColdAmount;
        public float maxColdAmount;

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

            //set cold meter variables
            currentColdAmount = maxColdAmount;
            coldMeter.SetMaxCold(maxColdAmount);
            coldMeter.SetCurrentCold(currentColdAmount);

        }

        void Update()
        {
            //reduce colde meter
            currentColdAmount -= coldIntensity * Time.deltaTime;
            //coldMeter.DepleteMeter(currentColdAmount);
            coldMeter.SetCurrentCold(currentColdAmount);
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
