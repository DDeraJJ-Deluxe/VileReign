using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    private PlatformEffector2D effector;
    private float waitTime = 0.3f;  
    void Start() {
        effector = GetComponent<PlatformEffector2D>();
    }
    
    void Update() {
        if (Input.GetKey(KeyCode.S)) {
            effector.rotationalOffset = 180f;
            Invoke("ResetPlatformEffector", waitTime);
        }
    }

    void ResetPlatformEffector() {
        effector.rotationalOffset = 0f;
    }
}