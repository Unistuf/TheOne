using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayTimer : MonoBehaviour
{
    public GameObject player;

    TextMeshProUGUI timeText;
    float elapsedTime;

    void start()
    {
        // Player = GameObject.Find("Player");
        // Health = player.GetComponent<Player Health>();
    }

    // Update is called once per frame
    void Update()
    {
       /* debug.log("health");*/
    /*    if (health <= 0)
        {




        }
        else
        {
            elapsedTime += Time.deltaTime * 60;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            timeText.text = string.Format("{00:00:00}:{1:00}", minutes, seconds);
        }*/

        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timeText.text = string.Format("{00:00:00}:{1:00}", minutes, seconds);
    }
}
