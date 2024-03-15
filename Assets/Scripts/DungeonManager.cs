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

    [Header("SafeZones")]
    public Vector2 safeZoneMinMax;
    
    [Header("Rooms")]
    public dungeonTile[] dungeonTiles;        // 1+
    public dungeonTile[] safeZoneTiles;       // -3
    public GameObject dungeonRoomExitPrefab;  // -2
    public GameObject dungeonRoomEntryPrefab; // -1 

    [Header("Runtime")]
    public int[,] dungeonLayout;
    public List<GameObject> spawnedTiles;
    private GameObject temp;
    private DungeonTileScript tempTileScript;

    void Start()
    {
        GenerateDungeon();
    }

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
        MakeSafeZone();

        //Left side code
        for (int i = midpoint - 1; i > 0; i--)
        {
            for (int y = 1; y < gridLength - 1; y++)
            {
                if (dungeonLayout[i + 1, y] != 0 && Random.Range(0,100) <= roomChance && dungeonLayout[i + 1, y] != -3) //Only continue if the tile is filled (no safezones) and if the chance is met
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
                if (dungeonLayout[i - 1, y] != 0 && Random.Range(0,100) <= roomChance && dungeonLayout[i - 1, y] != -3) //Only continue if the tile is filled (no safezones) and if the chance is met
                {
                    dungeonLayout[i, y] = Random.Range(1, dungeonTiles.Length); //Pick from tiles
                }
            }           
        }

        CheckIfSafeZonesConnected();
    }

    void CheckIfSafeZonesConnected() //Check if the tile is a safezone and delete it if its not attached to a room (safe rooms do not count as a connection)
    {
        bool isTileConnected = false;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                if (dungeonLayout[x, y] == -3) 
                {
                    if (y < gridLength) //Check if tile is below
                    {
                        if (dungeonLayout[x, y + 1] != 0 && dungeonLayout[x, y + 1] != -3)
                        {
                            isTileConnected = true;
                        }
                        else if (x == gridWidth / 2)
                        {
                            isTileConnected = true;
                        }
                    }

                    if (y > 0) //Check if tile is above
                    {
                        if (dungeonLayout[x, y - 1] != 0 && dungeonLayout[x, y - 1] != -3)
                        {
                            isTileConnected = true;
                        }
                        else if (x == gridWidth / 2)
                        {
                            isTileConnected = true;
                        }
                    }

                    if (!isTileConnected)
                    {
                        dungeonLayout[x, y] = 0;
                    }                    
                }
            }
        }
    }

    void MakeSafeZone()
    {
        for (int i = 0; i < Random.Range(safeZoneMinMax.x, safeZoneMinMax.y); i++)
        {
            int rngX = Random.Range(1, gridWidth - 1); //Random X pos
            int rngY = Random.Range(1, gridLength - 1); //Random Y pos

            if (i < safeZoneMinMax.x) //Ensure a minimum amount of Safe Zones are placed in the center 
            {
                rngX = gridWidth / 2;
            }
      
            dungeonLayout[rngX, rngY] = -3; //Make tile a safezone
        }
    }

    void MakeDefinatePath(int midpoint) //Makes the middle path
    {
        dungeonLayout[midpoint, gridLength - 1] = -2; //Set to -2 which refers to exit tile
        dungeonLayout[midpoint, 0] = -1; //Set to -1 which refers to entry tile

        for (int i = 1; i < gridLength - 1; i++)
        {
            if (dungeonLayout[midpoint, i] == 0)
            {
                dungeonLayout[midpoint, i] = Random.Range(1, dungeonTiles.Length); //Pick from tiles
            }
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
                        
                        tempTileScript = temp.GetComponent<DungeonTileScript>();
                        CheckForTiles(x, y, tempTileScript); //Start the tile check on each side
                    break;

                    case 0: break; //Empty tile

                    case -1: //Start Tile
                        temp = Instantiate(dungeonRoomEntryPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); 
                        temp.transform.parent = this.gameObject.transform;
                        GameObject.Find("Player").transform.position = new Vector3(x * 10, y * 10, 0);
                    break; 

                    case -2: //End Tile
                        temp = Instantiate(dungeonRoomExitPrefab, new Vector3(x * 10, y * 10, 0), transform.rotation); 
                        temp.transform.parent = this.gameObject.transform;
                    break; 

                    case -3: //Safe zone tile
                        temp = Instantiate(safeZoneTiles[Random.Range(0, safeZoneTiles.Length)].prefab, new Vector3(x * 10, y * 10, 0), transform.rotation); 
                        temp.transform.parent = this.gameObject.transform;

                        tempTileScript = temp.GetComponent<DungeonTileScript>();
                        CheckForTiles(x, y, tempTileScript); //Start the tile check on each side
                    break; 
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
