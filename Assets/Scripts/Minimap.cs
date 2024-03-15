using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject miniMapCamera;

    [Header("Config")]
    public float iconDistance;
    public RenderTexture minimapTexture;
    public Transform playerIcon;
    public Transform player;
    
    [Header("Prefabs")]
    public GameObject entryIcon;
    public GameObject exitIcon;
    public GameObject regularIcon;
    public GameObject safeZoneIcon;

    [Header("Runtime")]
    public List<GameObject> spawnedIcons;
    private GameObject temp;
    private int savedGridLength = 0;
    private int savedGridWidth = 0;


    void Update()
    {
            float x = transform.position.x + ((player.transform.position.x / 10) * iconDistance);
            float y = transform.position.y + ((player.transform.position.y / 10) * iconDistance);

            playerIcon.position = new Vector3(x, y, -1.5f);

            minimapTexture.Release();
    }

    public void UpdateMap(int[,] dungeonLayout, int gridLength, int gridWidth)
    {
        savedGridLength = gridLength;
        savedGridWidth = gridWidth;

        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            Destroy(spawnedIcons[i]);
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                switch(dungeonLayout[x,y])
                {
                    default:
                        temp = Instantiate(regularIcon, new Vector3(transform.position.x + x * iconDistance, transform.position.y + y * iconDistance, 0), transform.rotation);
                        spawnedIcons.Add(temp);
                        temp.transform.SetParent(transform); 
                    break;

                    case 0: break;

                    case -1: 
                        temp = Instantiate(entryIcon, new Vector3(transform.position.x + x * iconDistance, transform.position.y + y * iconDistance, 0), transform.rotation);
                        spawnedIcons.Add(temp);
                        temp.transform.SetParent(transform); 
                    break;

                    case -2: 
                        temp = Instantiate(exitIcon, new Vector3(transform.position.x + x * iconDistance, transform.position.y + y * iconDistance, 0), transform.rotation);
                        spawnedIcons.Add(temp);
                        temp.transform.SetParent(transform);               
                    break;

                    case -3: 
                        temp = Instantiate(safeZoneIcon, new Vector3(transform.position.x + x * iconDistance, transform.position.y + y * iconDistance, 0), transform.rotation);
                        spawnedIcons.Add(temp);
                        temp.transform.SetParent(transform);               
                    break;
                }
            }
        }


        Vector3 newCamPosition = new Vector3(transform.position.x + ((gridWidth * iconDistance) / 2), transform.position.y + ((gridLength * iconDistance) / 2), -5);

        miniMapCamera.transform.position = newCamPosition;
        miniMapCamera.GetComponent<Camera>().orthographicSize = (gridLength * 1.2f) + (gridWidth * 0.1f);
        minimapTexture.Release();
    }
}
