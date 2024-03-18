using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public GameObject player;
    public PlayerHealth health;

    [SerializeField] TextMeshProUGUI timeText;
    float elapsedTime;

    void Start()
    {
        player = GameObject.Find("Player");
        health = player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (health.currentHealth <= 0)
        {

        }
        else
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            timeText.text = string.Format("{00:00}:{1:00}", minutes, seconds);
        }*/
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timeText.text = string.Format("{00:00}:{1:00}", minutes, seconds);
    }
}
