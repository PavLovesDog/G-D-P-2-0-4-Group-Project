using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MBF
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health Variables")]
        public float currentHealth;
        public float maxHealth;
        bool deathCheck;
        bool displayHealthBar;
        float healthVisibilityCounter;
        float healthBarAlpha;

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
        EnemyStats enemyStats;
        GameManager gameManager;
        // Reference to animator handler to play damage and death animations??

        private void Awake()
        {
            healthBar = FindObjectOfType<HealthBar>();
            coldMeter = FindObjectOfType<ColdMeter>();
            playerMovement = GetComponent<PlayerMovement>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            gameManager = FindObjectOfType<GameManager>();
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
            if (!gameManager.gamePaused)
            {
                HandleWarmingAndFreezingStates();
                HandleHealthBarVisuals();

                //visually track changes
                coldMeter.SetCurrentCold(currentColdAmount);
                healthBar.SetCurrenthealth(currentHealth);

                //death check
                {
                    if (currentHealth <= 0) // Death condition
                    {
                        currentHealth = 0;
                        //Play death animation
                        if (deathCheck)
                        {
                            deathCheck = false;
                            animatorHandler.PlayTargetAnimation("Death");
                        }
                        //stop move functions
                        playerMovement.isDead = true;
                    }
                }
            }
        }

        private void HandleHealthBarVisuals()
        {
            //display health meter
            if (displayHealthBar)
            {
                healthVisibilityCounter += Time.deltaTime; // 
                healthBarAlpha = Mathf.InverseLerp(0, 1, healthVisibilityCounter);

                Image[] sprites = gameManager.healthBar.GetComponentsInChildren<Image>();
                foreach (Image sprite in sprites)
                {
                    sprite.enabled = true; // enable them
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, healthBarAlpha);
                }

                //start coroutine to flip bool
                StartCoroutine(DisplayHealthBar());
            }
            else
            {
                healthVisibilityCounter -= Time.deltaTime / 3;
                healthVisibilityCounter = Mathf.Clamp01(healthVisibilityCounter); // clamp it so it stays within 0 to 1 range
                healthBarAlpha = Mathf.InverseLerp(0, 1, healthVisibilityCounter);

                Image[] sprites = gameManager.healthBar.GetComponentsInChildren<Image>(); // get components of health bar.
                foreach (Image sprite in sprites)
                {
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, healthBarAlpha);
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
                //currentHealth -= coldIntensity * Time.deltaTime;
                DoDamage((int)coldIntensity);
            }
            else
            {
                isFreezing = false;
                playerMovement.moveSpeed = 5f;
            }
        }

        public void DoDamage(int damage)
        {
            //show health bar for short period of time
            displayHealthBar = true;

            currentHealth -= damage * Time.deltaTime; // minus damage
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

        IEnumerator DisplayHealthBar()
        {
            yield return new WaitForSeconds(1);
            displayHealthBar = false;
        }
    }
}
