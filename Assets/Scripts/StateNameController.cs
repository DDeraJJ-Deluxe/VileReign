using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateNameController : MonoBehaviour
{

    string healthKey = "Health";
    string levelKey = "Level";

    public int CurrentHealth {get; set;}

    public int CurrentLevel { get; set;}

    private void Awake()
    {
        CurrentHealth = PlayerPrefs.GetInt(healthKey);
        CurrentHealth = PlayerPrefs.GetInt(levelKey);
    }

    public void SetHealth(int health)
    {
        PlayerPrefs.SetInt(healthKey, health);
    }

    public void SetLevel(int level)
    {
        PlayerPrefs.SetInt(levelKey, level);
    }
}
