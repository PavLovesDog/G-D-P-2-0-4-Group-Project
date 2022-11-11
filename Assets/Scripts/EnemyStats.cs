using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MBF
{
    public class EnemyStats : MonoBehaviour
    {
        public enum EnemyType
        {
            Wolf,
            Bear
        }

        public EnemyType enemyType;

        public float currentHealth;
        public float maxHealth;
        public int strength;
        public int damageMultiplier;

        [Header("Health Bar")]
        public float width;
        public float height;
        public float xPos;
        public float yPos;

        float soundTimer;
        bool isTakingDamage;
        bool canDoDamage;
        public bool hasHit;

        public GameObject hitparticle;

        PlayerStats playerStats;
        ThrowRock throwRock;
        EnemyManager enemyManager;
        NavMeshAgent navMeshAgent;
        AudioManager audioManager;
        AudioSource enemyAudioSource;
        Rigidbody rb;
        

        void Start()
        {
            soundTimer = 7;
            canDoDamage = true;
            playerStats = FindObjectOfType<PlayerStats>();
            throwRock = FindObjectOfType<ThrowRock>(); // get reference for damage
            audioManager = FindObjectOfType<AudioManager>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyManager = GetComponent<EnemyManager>();
            enemyAudioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // track enemy health depletion, for smooth visual deletion of health
            if(hasHit)
            {
                playerStats.DoDamage(strength * damageMultiplier);
            }

            if(enemyManager.foundPlayer && !enemyManager.isDead)
            {
                soundTimer += Time.deltaTime;
                if(soundTimer > 7)
                {
                    soundTimer = 0;
                    //play the angry sounds
                    audioManager.PlayAudio(audioManager.wolfPursuit, audioManager.angryWolf, 0.35f);
                }
            }
            else
            {
                soundTimer = 7;
            }
        }

        //// Draw health bar on screen
        private void OnGUI()
        {
            Vector2 targetPos;
            targetPos = Camera.main.WorldToScreenPoint(transform.position);

            if (isTakingDamage && !enemyManager.isDead)
            {
                GUI.Box(new Rect(targetPos.x - xPos, Screen.height - targetPos.y - yPos, width, height), currentHealth + "/" + maxHealth);
            }
        }

        //Handle collisions
        private void OnCollisionEnter(Collision collision)
        {
            //if enemy collides with the player
            if (collision.collider.tag == "Player" && canDoDamage && !enemyManager.isDead) // touched an enemy!
            {
                Debug.Log(gameObject.name + " hit " + collision.gameObject.name);

                //Play audio of hit!
                audioManager.PlayAudio(audioManager.physicalHitSource, audioManager.impact_2, 0.45f);

                Instantiate(hitparticle, collision.transform.position + new Vector3(0,1,0), Quaternion.identity);
                canDoDamage = false;
                hasHit = true;

                StartCoroutine(DealDamageToPlayer());
            }

            //if a thrown rock hits the Enemy
            if(collision.gameObject.tag == "Projectile")
            {
                Debug.Log("HIT " + gameObject.name + "!");

                //Play audio of hit!
                audioManager.PlayAudio(audioManager.projectileHitSource, audioManager.impact, 0.45f);

                isTakingDamage = true;
                Instantiate(hitparticle, collision.transform.position, Quaternion.identity);

                if (!enemyManager.isDead)
                {
                    if (enemyType == EnemyType.Bear)
                    {
                        TakeDamage(throwRock.damage / 2);
                    }
                    else
                    {
                        TakeDamage(throwRock.damage);
                    }

                    StartCoroutine(DealDamageToSelf());
                }
            }

            //if enemy is hit by companion
            if (collision.gameObject.tag == "Companion")
            {
                Debug.Log("Companion BIT " + gameObject.name + "!");

                //Play audio of hit!
                audioManager.PlayAudio(audioManager.companionHitSource, audioManager.impact_2, 0.35f);

                isTakingDamage = true;
                //instantiate a hit particle
                Instantiate(hitparticle, transform.position, Quaternion.identity);

                if (!enemyManager.isDead)
                {
                    //reduce health
                    if (enemyType == EnemyType.Bear)
                    {
                        TakeDamage(5);
                    }
                    else
                    {
                        TakeDamage(10);
                    }

                    StartCoroutine(DealDamageToSelf());
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (isTakingDamage)
                currentHealth -= damage; // minus damage

            if (currentHealth <= 0) // Death condition
            {
                currentHealth = 0;
                //Play death animation ?
                Destroy(navMeshAgent);

                //stop move functions
                enemyManager.isDead = true;
                // play a death sound!
                enemyAudioSource.loop = false;
                enemyAudioSource.volume = 0.65f;
                enemyAudioSource.PlayOneShot(audioManager.wolfDeath); // for now stop the growling

                audioManager.StopSound(audioManager.wolfPursuit); // stop pursuit sounds
                Debug.Log("You killed a " + gameObject.name + "!");
            }
        }

        IEnumerator DealDamageToPlayer()
        {
            yield return new WaitForSeconds(0.5f); // pause for a moment
            hasHit = false;
            canDoDamage = true;
        }

        IEnumerator DealDamageToSelf()
        {
            yield return new WaitForSeconds(0.5f); // pause for a moment
            isTakingDamage = false;
            //canDoDamage = true;
        }
    }
}
