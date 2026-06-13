using UnityEngine;
namespace Plane
{
    public class Health : MonoBehaviour
    {
        public Transform model;
        public Transform huskPrefab;
        public float maxHealth;
        private float currentHealth;
        private bool dead;
        public bool IsDead {  get { return dead; } }
        private float CalculateDamagePerSecond(float damage)
        {
            float dt = Time.deltaTime;
            float seconds = dt;
            seconds += dt;
            Debug.Log(seconds);

            return (damage * seconds / 60);
        }
        private void OnDisable()
        {
            //HuskPrefab Inherits Velocity
            
        }
        public void ApplyDamage(float damage)
        {
        
            float DamageRate = CalculateDamagePerSecond(damage);
            currentHealth -= DamageRate;
          

        }
        void CheckDead()
        {
            dead = currentHealth < 0;
        }

        public void SpawnHusk()
        {
            if(dead)
            {
               model.gameObject.SetActive(false);
               Destroy(gameObject,1);
            }
        }
        private void LateUpdate()
        {
            CheckDead();
            SpawnHusk();
            Debug.Log(currentHealth);
        }
        private void Start()
        {
            currentHealth = maxHealth;
        }



    }
}
