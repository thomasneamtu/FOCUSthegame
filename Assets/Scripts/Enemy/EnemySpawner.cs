using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{   
    
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float spawnSpeed = 5f;
    [SerializeField] private bool spawning = true;
    [SerializeField] private List<GameObject> currentEnemies = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EnemySpawnOnProgress());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnemySpawnOnProgress()
    {
        while (spawning)
        {
            yield return new WaitUntil(() => currentEnemies.Count == 0);

            float progress = player.position.z;
            int enemiesThisWave = Mathf.Clamp(Mathf.FloorToInt(progress / 20f) + 1, 1, 4);

            currentEnemies.Clear();

            for (int i = 0; i < enemiesThisWave; i++)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Length);
                Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnSpeed);
        }
    }

    bool AllEnemiesAreDead()
    {
        currentEnemies.RemoveAll(enemyPrefab => enemyPrefab == null);

        return currentEnemies.Count == 0;
    }

    public void StopSpawning()
    {
        spawning = false;
    }
}
