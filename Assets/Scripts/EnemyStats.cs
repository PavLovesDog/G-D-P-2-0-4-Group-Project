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

        bool canDoDamage;
        public bool hasHit;

        PlayerStats playerStats;
        ThrowRock throwRock;
        EnemyManager enemyManager;
        NavMeshAgent navMeshAgent;
        

        void Start()
        {
            canDoDamage = true;
            playerStats = FindObjectOfType<PlayerStats>();
            throwRock = FindObjectOfType<ThrowRock>(); // get reference for damage
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyManager = GetComponent<EnemyManager>();
            currentHealth = maxHealth;
        }

        //Handle collisions
        private void OnCollisionEnter(Collision collision)
        {
            //if enemy collides with the player
            if (collision.collider.tag == "Player" && canDoDamage && !enemyManager.isDead) // touched an enemy!
            {
                Debug.Log(gameObject.name + " hit " + collision.gameObject.name);
                canDoDamage = false;
                hasHit = true;
                //navMeshAgent.isStopped = true; // stop them for a second THIS AINT WORKING
                StartCoroutine(DealDamageToPlayer());
            }

            //if a thrown rock hits the Enemy
            if(collision.gameObject.tag == "Projectile")
            {
                Debug.Log("HIT " + gameObject.name + "!");
                if(enemyType == EnemyType.Bear)
                {
                    TakeDamage(throwRock.damage / 2);
                }
                else
                {
                    TakeDamage(throwRock.damage);
                }
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage; // minus damage

            if (currentHealth <= 0) // Death condition
            {
                currentHealth = 0;
                //Play death animation ?
                //stop move functions
                enemyManager.isDead = true;
                Debug.Log("You killed a " + gameObject.name + "!");
            }
        }

        IEnumerator DealDamageToPlayer()
        {
            
            playerStats.DoDamage(strength * damageMultiplier); // call to damage playeer
            yield return new WaitForSeconds(1);
            //navMeshAgent.isStopped = false;
            hasHit = false;
            canDoDamage = true;
        }
    }
}
