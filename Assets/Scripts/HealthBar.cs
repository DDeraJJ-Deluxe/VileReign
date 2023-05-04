using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public Slider slider;

    private Queue<int> healthQueue = new Queue<int>();
    private bool isProcessingHealth = false;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value  = health;
    }

    public void SetHealth(int health)
    {
        healthQueue.Enqueue(health);
        if (!isProcessingHealth) {
            StartCoroutine(ProcessHealthQueue());
        }
    }

    private IEnumerator ProcessHealthQueue() {
        isProcessingHealth = true;
        while (healthQueue.Count > 0) {
            int health = healthQueue.Dequeue();
            slider.DOValue(health, 0.25f);
            yield return new WaitForSeconds(0.25f);
        }
        isProcessingHealth = false;
    }
    
}
