using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance {get; private set;}

    [SerializeField] List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] Queue<Enemy> waitingEnemies = new Queue<Enemy>();

    private Enemy currentFighter;
    [SerializeField] private int maxFighters = 1;

    private void Awake()
    {
        if(instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

   public void RegisterEnemy(Enemy enemy)
   {
        if(!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
   }

   public void UnregisterEnemy(Enemy enemy)
   {
        activeEnemies.Remove(enemy);
        if(waitingEnemies.Contains(enemy))
        {
            Queue<Enemy> newQueue = new Queue<Enemy>();
            foreach (var e in waitingEnemies)
            {
                if (e != enemy) newQueue.Enqueue(e);
            }
            waitingEnemies = newQueue;
        }

        if(currentFighter == enemy)
        {
            currentFighter = null;  
        }
   }

    public void RequestAttackTurn(Enemy enemy)
    {
        if(currentFighter == null)
        {
            currentFighter = enemy;
            enemy.StartAttack();
        }
        else
        {
            if(!waitingEnemies.Contains(enemy))
            {
                waitingEnemies.Enqueue(enemy);
            }
        }
    }

    public void OnEnemyDone(Enemy enemy)
    {
        if(currentFighter == enemy)
        {
            currentFighter = null;
        }
        if(waitingEnemies.Count > 0)
        {
            var nextEnemy = waitingEnemies.Dequeue();
            currentFighter = nextEnemy;
            nextEnemy.StartAttack();
        }    
    }
}
