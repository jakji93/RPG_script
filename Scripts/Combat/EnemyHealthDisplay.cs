using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTarget() != null)
            {
                Health health = fighter.GetTarget();
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoint(), health.GetMaxHitPoint()); 
            }
            else
            {
                GetComponent<Text>().text = "N/A";
            }
        }
    } 
}
