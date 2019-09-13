using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;
        // Update is called once per frame
        void Update()
        {
            if (healthComponent.GetAlreadyDead() || Mathf.Approximately(healthComponent.GetPercentage()/100, 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetPercentage() / 100, 1, 1);
        }
    } 
}
