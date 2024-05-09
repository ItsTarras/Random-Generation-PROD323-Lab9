using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PoissonDiskSpawner : MonoBehaviour
{
    // The radius from the center of an object
    public float diskRadius;

    // Max and min spawn range on the terrain
    [Range(0f, 150f)]
    public float spawnRadiusMax = 100f;

    [Range(0f, 150f)]
    public float spawnRadiusMin = 20f;

    // Additional padding between disks
    [Range(2, 50)]
    public float separationMax = 20;

    [Range(2, 50)]
    public float separationMin = 5;

    // Max number of tries placing an object before moving to the next one
    public float maxTries;
    public Terrain terrain;

    // Stores objects that are to be generated
    public GameObject[] objects;
    public GameObject house;

    // Stores disk/object positions
    List<GameObject> activeList = new List<GameObject>();

    GameObject currentDisk;

    GameObject Spawner;

    // Contains the hypothetical grid cells
    // If the cell is no longer available for placement, value would then be true. 
    bool[,] diskGrid;
    int gridSize;

    void Start()
    {
        Spawner = gameObject;
        // Create the hypothetical grid to keep track of valid tiles to spawn an object
        // Center is an odd number
        gridSize = (int) spawnRadiusMax * 2;
        diskGrid = new bool[gridSize, gridSize];

        // Once the grid size is setup, generate the objects
        GenerateObjects();
    }

    public void GenerateObjects(){
        // STEP 1: Make sure to reset everything
        // Destroy all spawner children and clear the activeList
        activeList.Clear();
        foreach (Transform child in transform){
            Destroy(child.gameObject);
        }

        // TODO: Reset all disk cells to false
        for (int i = 0; i < gridSize; i++){
            for (int j = 0; j < gridSize; j++){
                diskGrid[i, j] = false;
            }
        }

        // STEP 2: Once everything is reset, add the first disk/object in the activeList
        // TODO: The first object is the spawner itself
        activeList.Add(gameObject);

        // STEP 3: Calculate the position of the next disk
        while (activeList.Count > 0){
            bool success = false;
            currentDisk = activeList[activeList.Count-1];

            for(int i = 0; i < maxTries; i++){
                // TODO: Calculate the coordinate of the next disk using polar coordinates
                // Refer to lecture slides
                //Debug.Log($"{i}/{maxTries} = " + (float)i / maxTries);
                float angle = i / maxTries * 2 * Mathf.PI;
                //Debug.Log(angle);
                float radius = 2f * diskRadius + Random.Range(separationMin, separationMax);
                float xOffset = Mathf.Cos(angle) * radius;
                float zOffset = Mathf.Sin(angle) * radius;
                float xCoord = currentDisk.transform.position.x + xOffset;
                float zCoord = currentDisk.transform.position.z + zOffset;

                Vector3 pos = new Vector3(xCoord, 0, zCoord);

                float dist = Vector3.Distance(pos, Spawner.transform.position);
                // TODO: Check if the new position is within the spawner bounds
                // If true, then place the disk/object on terrain
                if (dist < spawnRadiusMax && spawnRadiusMin < dist)
                {
                    //Place the object.
                    success = PlaceDisk(xCoord, zCoord);
                }
 

                // If placement is successful, then do the next disk/object in the activeList
                if(success)
                    break;
            }

            // If all available tries were spent and placement was not successful,
            // then remove the disk/object from activeList. If there are no more disk/objects in the 
            // activeList, finish the whole operation.
            if (!success){
                activeList.RemoveAt(activeList.Count - 1);
                if (activeList.Count == 0)
                    break;
            }
        }

    }

    // This function places the object in the hypothetical grid.
    // Returns a boolean value that represents the success status of the operation
    private bool PlaceDisk(float xCoord, float zCoord){
        bool valid = true;
        
        // Calculate disk coord
        int x = (int) (xCoord) / (int) diskRadius;
        int y = (int) (zCoord) / (int) diskRadius;

        // Check if the current cell in the diskgrid is available, if not then the whole place is invalid
        if (diskGrid[x, y])
        {
            valid = false;
        }

        // If the current diskgrid is available, then place the object
        if(valid){
            // Since this placement is valid, we should now make sure to note
            // that this particular cell in the diskgrid is no longer available
            diskGrid[x, y] = true;

            // TODO: For every valid placement, a house has a 1% chance to spawn. 
            // If a house has no chance to spawn, randomly generate vegetation/terrain.
            // House and objects should randomly rotate to make it more visually appealing.
            // Add the newly spawned object in the activeList to continue generation.
            bool randomChance = (Random.Range(1, 101) == 1);
            GameObject spawnedObject;
            if (randomChance)
            {
                //Spawn a house. 
                spawnedObject = Instantiate(house, new Vector3(xCoord, 0, zCoord), Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
            else
            {
                spawnedObject = Instantiate(objects[Random.Range(0, objects.Length)], new Vector3(xCoord, 0, zCoord), Quaternion.identity);
            }

            activeList.Add(spawnedObject);
        }
        
        return valid;
    }

}
