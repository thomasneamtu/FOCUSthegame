using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;

    private enum State { Waiting, Approaching, Attacking, Retreating }
    private State currentState = State.Waiting;

    public float approachDistance = 2.5f;
    public float retreatDistance = 3f;
    public float attackDuration = 1.2f;

    [SerializeField] private float speed;
    [SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        speed = agent.velocity.magnitude;

        
        EnemyManager.instance.RegisterEnemy(this);

        StartCoroutine(BehaviorLoop());
    }

    public void StartAttack()
    {
        StartCoroutine(AttackSequence());
    }    
   
    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            yield return null;

            if (currentState == State.Waiting)
            {
                EnemyManager.instance.RequestAttackTurn(this);
            }
        }
    }

    IEnumerator AttackSequence()
    {
        currentState = State.Approaching;
        animator.SetBool("Approaching", true);
        agent.isStopped = false;
        agent.SetDestination(player.position);

        while(Vector3.Distance(transform.position,player.position) > approachDistance)
        {
            yield return null;
        }

        agent.isStopped = true;
        currentState = State.Attacking;
        animator.SetBool("Approaching", false);
        animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(attackDuration);

        currentState = State.Retreating;
        animator.SetBool("Attacking", false);
        animator.SetBool("Retreating", true);
        Vector3 retreatDir = (transform.position = player.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDir * retreatDistance;
        
        agent.SetDestination(retreatTarget);

        while (Vector3.Distance(transform.position, retreatTarget) > 0.5f)
        {
            yield return null;  
        }

        agent.isStopped = true;
        currentState = State.Waiting;
        animator.SetBool("Retreating", false);
        animator.SetBool("Waiting", true);
        EnemyManager.instance.OnEnemyDone(this);
    }

    private void OnDeath()
    {
        if(EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(this);
        }
    }
}
