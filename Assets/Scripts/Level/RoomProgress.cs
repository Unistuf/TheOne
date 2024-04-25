using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomProgress : MonoBehaviour
{
    public int nextSceneNumber;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player") //Save the player progression then move to the next room
        {
            other.gameObject.GetComponent<PlayerHealth>().SaveGame();
            SceneManager.LoadScene(nextSceneNumber);
        }
    }
}
