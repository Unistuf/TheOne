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
    public Minimap minimap;

    [Range(0,100)]
    public int roomChance;
    
    [Header("Rooms")]
    public dungeonTile[] dungeonTiles;
    public GameObject dungeonRoomExitPrefab;
    public GameObject dungeonRoomEntryPrefab;

    [Header("Runtime")]
    public int[,] dungeonLayout;
    public List<GameObject> spawnedTiles;
    private GameObject temp;

    void Update()
    {
        if (DebugGenerate){DebugGenerate = false; GenerateDungeon();} //Start the dungeon gen chain

        if (DebugPrintLayout){DebugPrintLayout = false; PrintLayout();} //Print the layout to the debug console
    }


    public void GenerateDungeon()
    {
        int midpoint = Mathf.FloorToInt(gridWidth / 2); //get the center point of the room

        ClearDungeon();
        GenerateDungeonLayout(midpoint);
        GenerateDungeonObjects();
        minimap.UpdateMap(dungeonLayout, gridLength, gridWidth);
    }

    void ClearDungeon()//Delete All previously placed tiles and data if they exist
    {
        for (int i = 0; i < spawnedTiles.Count; i++) 
        {
            Destroy(spawnedTiles[i]);
        }
        dungeonLayout = new int[gridWidth, gridLength];
        spawnedTiles = new List<GameObject>();
    }

    void GenerateDungeonLayout(int midpoint) //Generates the layout branching from the middle
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

    void MakeDefinatePath(int midpoint) //Makes the middle path
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
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                switch(dungeonLayout[x,y]) //gets the value of the tile at x/y
                {
                    default: //Default if its not an Empty or Start or End tile
                        temp = Instantiate(dungeonTiles[Random.Range(0, dungeonTiles.Length)].prefab, new Vector3(x * 10, y * 10, 0), transform.rotation); 
                        temp.transform.parent = this.gameObject.transform;
                        
                        DungeonTileScript tempTileScript = temp.GetComponent<DungeonTileScript>();
                        CheckForTiles(x, y, tempTileScript); //Start the tile check on each side
                    break;

                    case 0: break; //Empty tile
                    case -1: temp = Instantiate(dungeonRoomEntryPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); break; //Start Tile
                    case -2: temp = Instantiate(dungeonRoomExitPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); break; //End Tile
                }

                spawnedTiles.Add(temp);
            }
        }

    }

    void CheckForTiles(int x, int y, DungeonTileScript tempTileScript) //Checks if theres a tile on a respective side then enables the door if needed
    {
        if (y > 0) //Check if tile is below
        {
            if (dungeonLayout[x, y - 1] != 0)
            {
                tempTileScript.isTileBelow = true;
            }
        }

        if (x > 0) //Check if tile is to the left
        {
            if (dungeonLayout[x - 1, y] != 0)
            {
                tempTileScript.isTileLeft = true;
            }
        }
        if (x < gridWidth - 1) //Check if tile is to the right
        {
            if (dungeonLayout[x + 1, y] != 0)
            {
                tempTileScript.isTileRight = true;
            }
        }

        if (y < gridLength) //Check if tile is to the above
        {
            if (dungeonLayout[x, y + 1] != 0)
            {
                tempTileScript.isTileAbove = true;
            }
        }

        tempTileScript.UpdateDoors();
    }

    void PrintLayout() //Debug command that outputs the layout to the console
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
