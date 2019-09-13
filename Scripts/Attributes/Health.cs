using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        LazyValue<float> hitPoint;
        bool alreadyDead = false;

        private void Awake()
        {
            hitPoint = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        private void RegenerateHealth()
        {
            hitPoint.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool GetAlreadyDead()
        {
            return alreadyDead;
        }

        public void  TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage " + damage);
            hitPoint.value = Mathf.Max(hitPoint.value - damage, 0);
            
            if(hitPoint.value == 0 && !alreadyDead)
            {
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if(experience == null)
            {
                return;
            }
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPoint()
        {
            return hitPoint.value;
        }

        public float GetMaxHitPoint()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * hitPoint.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            GetComponent<Animator>().SetTrigger("die");
            alreadyDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return hitPoint;
        }

        public void RestoreState(object state)
        {
            hitPoint.value = (float)state;
            if (hitPoint.value == 0 && !alreadyDead)
            {
                Die();
            }
        }
    } 
}
