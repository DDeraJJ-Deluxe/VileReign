using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaunch : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform launchPoint;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start() {
    }   

    // Update is called once per frame
    public void Attack() {
        if (playerController.unlockedSwordBeam) {
            StartCoroutine(DelayForAttack());
        }
    }

    private IEnumerator DelayForAttack() {
        yield return new WaitForSeconds(0.25f);
        Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
    }
}
