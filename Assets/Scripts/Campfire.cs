using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public GameObject offSprite;
    public GameObject onSprite;
    public GameObject usedSprite;

    private bool campfireStatus = false;
    private bool isUsed = false;
    public float campfireDuration;

    void Update()
    {
        if (!isUsed) //Set the sprites if the campire is not used
        {
            offSprite.SetActive(!campfireStatus);
            onSprite.SetActive(campfireStatus);
            usedSprite.SetActive(false);
        }
        else{ //Set the sprites if the campire is used
            offSprite.SetActive(false);
            onSprite.SetActive(false);  
            usedSprite.SetActive(true);          
        }
    }

    void OnCollisionEnter2D(Collision2D col)//Detect the player colliding with the campfire 
    {
        if (isUsed == false && col.gameObject.tag == "Player")
        {
            StartCoroutine(StartCampFire()); //Turn the campfire on
        }
    }

    IEnumerator StartCampFire() //Campfire duration loop
    {
        campfireStatus = true;
        yield return new WaitForSeconds(campfireDuration);
        campfireStatus = false;
        isUsed = true;
    }
}
