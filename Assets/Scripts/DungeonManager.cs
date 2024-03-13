using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class dungeonTile
{
    public GameObject prefab;
}

public class DungeonManager : MonoBehaviour
{
    [Header("Debug")]
    public bool DebugGenerate;
    public bool DebugPrintLayout;

    [Header("Settings")]
    public int gridLength;
    public int gridWidth;

    [Range(0,100)]
    public int roomChance;
    
    [Header("Rooms")]
    public dungeonTile[] dungeonTiles;
    public GameObject dungeonRoomExitPrefab;
    public GameObject dungeonRoomEntryPrefab;

    [Header("Runtime")]
    public int[,] dungeonLayout;
    public List<GameObject> spawnedTiles;

    void Update()
    {
        if (DebugGenerate){DebugGenerate = false; GenerateDungeon();}

        if (DebugPrintLayout){DebugPrintLayout = false; PrintLayout();}
    }


    public void GenerateDungeon()
    {
        int midpoint = Mathf.FloorToInt(gridWidth / 2);

        ClearDungeon();
        GenerateDungeonLayout(midpoint);
        GenerateDungeonObjects();
    }

    void ClearDungeon()
    {
        dungeonLayout = new int[gridWidth, gridLength];
        spawnedTiles = new List<GameObject>();
    }

    void GenerateDungeonLayout(int midpoint)
    {
        MakeDefinatePath(midpoint);    

        //Left side code
        for (int i = midpoint - 1; i > 0; i--)
        {
            for (int y = 1; y < gridLength - 1; y++)
            {
                if (dungeonLayout[i + 1, y] != 0 && Random.Range(0,100) <= roomChance)
                {
                    dungeonLayout[i, y] = Random.Range(1, dungeonTiles.Length); //Pick from tiles
                }
            }           
        }

        //Right side code
        for (int i = midpoint + 1; i < gridWidth; i++)
        {
            for (int y = 1; y < gridLength - 1; y++)
            {
                if (dungeonLayout[i - 1, y] != 0 && Random.Range(0,100) <= roomChance)
                {
                    dungeonLayout[i, y] = Random.Range(1, dungeonTiles.Length); //Pick from tiles
                }
            }           
        }
    }

    void MakeDefinatePath(int midpoint)
    {
        dungeonLayout[midpoint, gridLength - 1] = -2; //Set to -2 which refers to exit tile
        dungeonLayout[midpoint, 0] = -1; //Set to -1 which refers to entry tile

        for (int i = 1; i < gridLength - 1; i++)
        {
            dungeonLayout[midpoint, i] = Random.Range(1, dungeonTiles.Length); //Pick from tiles
        }
    }

    void GenerateDungeonObjects()
    {
        GameObject temp = new GameObject();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                switch(dungeonLayout[x,y])
                {
                    default: temp = Instantiate(dungeonTiles[Random.Range(0, dungeonTiles.Length)].prefab, new Vector3(x * 10, y * 10, 0), transform.rotation); break;
                    case 0: break;
                    case -1: temp = Instantiate(dungeonRoomEntryPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); break;
                    case -2: temp = Instantiate(dungeonRoomExitPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); break;
                }

                spawnedTiles.Add(temp);
            }
        }

    }

    void PrintLayout()
    {
        string debugOutput = "";

        for (int y = 0; y < gridLength; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {           
                debugOutput = debugOutput + "(" + dungeonLayout[x, y] + ") ";
            }

            debugOutput = debugOutput + "\n";
        }

        Debug.Log(debugOutput);
    }
}
