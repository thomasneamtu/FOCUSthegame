using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<SpawnRoom> enemySpawnRooms;
    // get player for triggers to stop spawns out of doors passed.

    [SerializeField] private int enemiesPerWave;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    [SerializeField] private float timeBetweenWaves = 0.5f;
    [SerializeField] private int totalWaves = 10;

    private int currentWave = 0;
    [SerializeField] private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {

            Transform spawnPoint = GetClosestValidSpawnPoint(enemySpawnRooms);
            if (spawnPoint == null)
            {
                Debug.Log("No Spawns Found, Stopping Spawns");
                yield break;
            }

            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        yield return new WaitUntil(() => EnemyManager.instance.activeEnemies.Count == 0);
       
        currentWave++;
        if (currentWave < totalWaves)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            StartCoroutine(SpawnWave());
        }
    }


    private Transform GetClosestValidSpawnPoint(List<SpawnRoom> SpawnRoomsCheck)
    {
        Transform closestPoint = null;
        float minDistance = Mathf.Infinity;
        bool hasActiveRooms = false;

        foreach(var room in SpawnRoomsCheck)
        {
            if (!room.isActive) continue;

            hasActiveRooms = true;

            Transform candidate = room.GetClosestSpawnPoint(player.position);
            if (candidate != null)
            {
                float dist = Vector3.Distance(candidate.position, player.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestPoint = candidate;
                }
            }

        }

        if (!hasActiveRooms)
        {
            Debug.LogWarning("No SpawnRooms Remain");
            return null;
        }

        if(closestPoint == null)
        {
            Debug.LogWarning("No SpawnRooms Found");
        }

        return closestPoint;
    }

    public void DeactivateRoom(SpawnRoom room)
    {
        room.isActive = false;
    }


}
