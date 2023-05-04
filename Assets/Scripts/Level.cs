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
        slider.DOValue(slider.value + exp, 0.5f);
        if (slider.value >= slider.maxValue) {
            lvlValue++;
            slider.value -= slider.maxValue;
            slider.maxValue += 50;
        }
    }
}
