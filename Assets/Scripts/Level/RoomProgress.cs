using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomProgress : MonoBehaviour
{
    public int nextSceneNumber;
    public int bossSceneNumber;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player") //Save the player progression then move to the next room
        {
            other.gameObject.GetComponent<PlayerHealth>().levelsCleared++;
            other.gameObject.GetComponent<PlayerHealth>().SaveGame();

            // If the player has cleared a multiple of 5 levels, take them to a boss level
            if (other.gameObject.GetComponent<PlayerHealth>().levelsCleared % 5 == 0)
            {
                SceneManager.LoadScene(bossSceneNumber);
            }
            else
            {
                SceneManager.LoadScene(nextSceneNumber);
            }
        }
    }
}
