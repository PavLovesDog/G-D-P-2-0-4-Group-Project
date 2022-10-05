using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{
    public class EnemyStats : MonoBehaviour
    {
        public int currentHealth;
        public int maxHealth;
        public int strength;
        public int damageMultiplier;

        bool canDoDamage;

        PlayerStats playerStats;

        void Start()
        {
            canDoDamage = true;
            playerStats = FindObjectOfType<PlayerStats>();
            currentHealth = maxHealth;
        }

        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Player" && canDoDamage) // touched an enemy!
            {
                canDoDamage = false;
                StartCoroutine(DealDamageToPlayer());
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
