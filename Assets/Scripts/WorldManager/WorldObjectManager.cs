using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    [Header("Object")]
    [SerializeField] List<ObjectSpawner> objectSpawners;
    [SerializeField] List<GameObject> spawnedInObjects;

    [Header("Fog Wall")]
    public List<FogWall> fogWalls;

    [Header("Braziers")]
    public List<BrazierInteractable> braziers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnObject(ObjectSpawner spawnedInObjects)
    {
        objectSpawners.Add(spawnedInObjects);
        spawnedInObjects.AttemptToSpawnObject();
    }

    public void AddFogWallToList(FogWall fw)
    {
        if (!fogWalls.Contains(fw))
        {
            fogWalls.Add(fw);
        }
    }

    public void RemoveFogWallFromList(FogWall fw)
    {
        if (fogWalls.Contains(fw))
        {
            fogWalls.Remove(fw);
        }
    }

    public void AddBrazierToList(BrazierInteractable bi)
    {
        if (!braziers.Contains(bi))
        {
            braziers.Add(bi);
        }
    }

    public void RemoveBrazierFromList(BrazierInteractable bi)
    {
        if (braziers.Contains(bi))
        {
            braziers.Remove(bi);
        }
    }

}
