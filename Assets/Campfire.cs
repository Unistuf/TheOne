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
        if (!isUsed)
        {
            offSprite.SetActive(!campfireStatus);
            onSprite.SetActive(campfireStatus);
            usedSprite.SetActive(false);
        }
        else{
            offSprite.SetActive(false);
            onSprite.SetActive(false);  
            usedSprite.SetActive(true);          
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isUsed == false && col.gameObject.tag == "Player")
        {
            StartCoroutine(StartCampFire());
        }
    }

    IEnumerator StartCampFire()
    {
        campfireStatus = true;
        yield return new WaitForSeconds(campfireDuration);
        campfireStatus = false;
        isUsed = true;
    }
}
