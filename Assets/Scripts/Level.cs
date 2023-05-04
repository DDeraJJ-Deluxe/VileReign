using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Level : MonoBehaviour
{

    public Slider slider;

    public static int lvlValue = 1;
    [SerializeField] public TextMeshProUGUI level;

    private Queue<int> expQueue = new Queue<int>();
    private bool isProcessingExp = false;

    void Start() {
        slider.value = 0;
        slider.maxValue = 100;
    }

    void Update() {
        level.text = lvlValue.ToString();
    }

    public void GainExp(int exp) {
        expQueue.Enqueue(exp);
        if (!isProcessingExp) {
            StartCoroutine(ProcessExpQueue());
        }
    }

    private IEnumerator ProcessExpQueue() {
        isProcessingExp = true;
        while (expQueue.Count > 0) {
            float currentExp = slider.value;
            int exp = expQueue.Dequeue();
            slider.DOValue(slider.value + exp, 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (currentExp + exp >= slider.maxValue) {
                float additionalExp = currentExp + exp - slider.maxValue;
                lvlValue++;
                slider.DOValue(0, 0.5f);
                yield return new WaitForSeconds(0.5f);
                slider.maxValue += 50;
                slider.DOValue(additionalExp, 0.5f);
            }
        }
        isProcessingExp = false;
    }
}
