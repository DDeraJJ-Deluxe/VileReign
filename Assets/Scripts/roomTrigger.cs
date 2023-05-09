using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
 
public class roomTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
 
    private void OnTriggerEnter2D(Collider2D other) {
        print("Trigger Entered");
        
        if(other.CompareTag("Player") && !other.isTrigger) {
            virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCam.Priority = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        print("Trigger Exited");
        
        if(other.CompareTag("Player") && !other.isTrigger) {
            virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCam.Priority = -1;
        }
    }
}