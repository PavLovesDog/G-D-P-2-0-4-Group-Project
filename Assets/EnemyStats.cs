using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        PlayerStats playerStats;
        ThrowRock throwRock;
        NPCDynamicPatrol nPC;

        void Start()
        {
            canDoDamage = true;
            playerStats = FindObjectOfType<PlayerStats>();
            throwRock = FindObjectOfType<ThrowRock>(); // get reference for damage
            nPC = GetComponent<NPCDynamicPatrol>();
            currentHealth = maxHealth;
        }

        void Update()
        {

        }

        //Handle collisions
        private void OnCollisionEnter(Collision collision)
        {
            //if enemy collides with the player
            if (collision.collider.tag == "Player" && canDoDamage) // touched an enemy!
            {
                canDoDamage = false;
                StartCoroutine(DealDamageToPlayer());
            }

            //if a thrown rock hits the Enemy
            if(collision.gameObject.tag == "Projectile")
            {
                Debug.Log("HIT!");
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
                nPC.npcIsDead = true;
            }
        }

        IEnumerator DealDamageToPlayer()
        {
            playerStats.DoDamage(strength * damageMultiplier); // call to damage playeer
            yield return new WaitForSeconds(1);
            canDoDamage = true;
        }
    }
}
