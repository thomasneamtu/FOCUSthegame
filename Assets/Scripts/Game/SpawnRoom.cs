using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{

    public List<Transform> spawnPoints = new List<Transform>();
    public bool isActive = true;

    public Transform GetClosestSpawnPoint(Vector3 playerPosition)
    {
        Transform closest = null;

        float closestSpawn = Mathf.Infinity;

        foreach (Transform sp in spawnPoints)
        {
            float dist = Vector3.Distance(playerPosition, sp.position);

            if (dist < closestSpawn)
            {
                closest = sp;
                closestSpawn = dist;
            }
        }

        return closest;
        
    }
}
