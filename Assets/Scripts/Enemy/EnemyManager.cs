using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }

    [SerializeField] public List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] public Queue<Enemy> waitingEnemies = new Queue<Enemy>();

    public int ActiveEnemyCount => activeEnemies.Count; 


    //[SerializeField] private int maxFighters = 1;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);

        instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            Debug.Log("Registered Enemy: {enemy.name}");
        }

    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

}
