using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class roomTrigger : MonoBehaviour
{
    public GameObject virtualCam;
 
    // Level move zoned enter, if collider is a player
    // Move game to another scene
    private void OnTriggerEnter2D(Collider2D other) {
        print("Trigger Entered");
        
        // Could use other.GetComponent<Player>() to see if the game object has a Player component
        // Tags work too. Maybe some players have different script components?
        if(other.tag == "Player" && !other.isTrigger) {
            // Player entered, so move level
            //print("Switching Scene to " + sceneBuildIndex);
            //SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        print("Trigger Entered");
        
        // Could use other.GetComponent<Player>() to see if the game object has a Player component
        // Tags work too. Maybe some players have different script components?
        if(other.tag == "Player" && !other.isTrigger) {
            // Player entered, so move level
            //print("Switching Scene to " + sceneBuildIndex);
            //SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
            virtualCam.SetActive(false);
        }
    }
}