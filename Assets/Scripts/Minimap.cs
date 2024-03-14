using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public GameObject miniMapCamera;

    [Header("Config")]
    public float iconDistance;
    public RenderTexture minimapTexture;
    
    [Header("Prefabs")]
    public GameObject entryIcon;
    public GameObject exitIcon;
    public GameObject regularIcon;

    [Header("Runtime")]
    public List<GameObject> spawnedIcons;
    private GameObject temp;


    public void UpdateMap(int[,] dungeonLayout, int gridLength, int gridWidth)
    {
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
                }
            }
        }


        Vector3 newCamPosition = new Vector3(transform.position.x + ((gridWidth * iconDistance) / 2), transform.position.y + ((gridLength * iconDistance) / 2), -1);

        miniMapCamera.transform.position = newCamPosition;
        miniMapCamera.GetComponent<Camera>().orthographicSize = (gridLength * 1.2f) + (gridWidth * 0.1f);
        minimapTexture.Release();
    }
}
