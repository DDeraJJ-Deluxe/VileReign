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

    void Start() {
        slider.value = 0;
        slider.maxValue = 100;
    }

    void Update() {
        level.text = lvlValue.ToString();
    }

    public void GainExp(int exp) {
        float currentExp = slider.value;
        slider.DOValue(slider.value + exp, 0.5f).OnComplete(() => {
            if (currentExp + exp >= slider.maxValue) {
                float additionalExp = currentExp + exp - slider.maxValue;
                lvlValue++;
                slider.DOValue(slider.value - slider.maxValue, 0.5f).OnComplete(() => {
                    slider.maxValue += 50;
                    slider.DOValue(slider.value + additionalExp, 0.5f);
                });
            }
        });
    }
}
