using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTileScript : MonoBehaviour
{
    [Header("Entry and Exits")]
    public GameObject northEntry;
    public GameObject eastEntry;
    public GameObject westEntry;
    public GameObject southEntry;

    [Header("Flags")]
    public bool isTileAbove = false;
    public bool isTileBelow = false;
    public bool isTileLeft = false;
    public bool isTileRight = false;

    public void UpdateDoors()
    {
        northEntry.SetActive(!isTileAbove);
        eastEntry.SetActive(!isTileRight);
        westEntry.SetActive(!isTileLeft);
        southEntry.SetActive(!isTileBelow);
    }
}
