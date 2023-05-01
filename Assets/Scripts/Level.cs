using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level : MonoBehaviour
{

    public static int lvlValue = 0;
    [SerializeField] private TextMeshProUGUI level;

    // Start is called before the first frame update
    void Start()
    {
        level = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "Level " + lvlValue;
    }
}
