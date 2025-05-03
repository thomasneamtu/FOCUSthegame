using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using UnityEngine.Experimental.AI;
using JetBrains.Annotations;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody[] ragdolls;
    [SerializeField] private NavMeshObstacle obstacle;

    [Header("Camera Effects")]
    private CinemachineImpulseSource impulseSource;

    private enum State { Waiting, Approaching, Attacking, Retreating, Stunned }
    private State currentState;

    [Header("Health and Hit Settings")]
    public int health = 4;
    private Vector3 koHitForce;
    private Vector3 koHitPoint;
    private FocusMode focusMode;

    [Header("EnemyAI Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 3f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isDead = false;

    //[SerializeField] private float attackCoolDown = 2f; maybe for WaitAndStrafe?

    [Header("Misc")]
    public float speed;

    

    private void Awake()
    {  
        ragdolls = GetComponentsInChildren<Rigidbody>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null )
        {
            obstacle.enabled = false;
        }

        DisableRagdoll();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.RegisterEnemy(this);
        }
        else
        {
            Debug.LogWarning("EnemyManager Instance was null when trying to register enemy.");
        }

        focusMode = GameObject.FindGameObjectWithTag("Player").GetComponent<FocusMode>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
      
        currentState = State.Approaching;
        
        StartCoroutine(BehaviorLoop());
    }


    private void Update()
    {
        speed = agent.velocity.magnitude; //tied to animation

        OnEnemyDead();
       
    }

    #region Health, Hit Detection and Ragdolls

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("PlayerHD"))
        {
            int damage = focusMode != null && focusMode.isFocusActive ? 2 : 1;
            TakeDamage(damage);
            
            animator.SetTrigger("Hit");
            
            koHitForce = collision.ClosestPoint(transform.position);
            koHitPoint = (transform.position - collision.transform.position).normalized * 0.5f;

            CameraShake.instance.CamShake(impulseSource);

            if(health <= 0 && focusMode != null && focusMode.isFocusActive)
            {
                ApplyFocusLaunchForce();
            }
        }
    }

    private void ApplyFocusLaunchForce()
    {
        EnableRagdoll();

        Rigidbody closestRb = FindHitRigidbody(koHitPoint);

        if (closestRb != null)
        {
            Vector3 launchForce = (transform.position - player.position).normalized * 165f;
            closestRb.AddForce(launchForce, ForceMode.Impulse);
        }

         
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    public void EnableRagdoll()
    {
        foreach (var rigidbody in ragdolls)
        {
            rigidbody.isKinematic = false;
        }

        SetDeadRagdollLayer(gameObject, LayerMask.NameToLayer("DeadRagdoll"));
        animator.enabled = false;
        characterController.enabled = false;
    }

    public void DisableRagdoll()
    {
        foreach (var rigidbody in ragdolls)
        {
            rigidbody.isKinematic = true;
        }
        animator.enabled = true;
        characterController.enabled = true;
    }

    public void SetDeadRagdollLayer(GameObject rag, int newLayer)
    {
        rag.layer = newLayer;
        foreach (Transform child in rag.transform)
        {
            SetDeadRagdollLayer(child.gameObject, newLayer);
        }
    }

    /*
    public void TriggerRigidbody(Vector3 hitForce, Vector3 hitPoint)
    {
        EnableRagdoll();

        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);

        Vector3 clampedForce = Vector3.ClampMagnitude(hitForce, 1f);

        hitRigidbody.AddForceAtPosition(clampedForce, hitPoint, ForceMode.Impulse);

    }
    */
     public Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closest = null;
        float minDist = float.MaxValue;

        foreach (var rb in ragdolls)
        {
            float dist = Vector3.Distance(rb.position, hitPoint);
            if (dist < minDist)
            {
                minDist = dist;
                closest = rb;
            }
        }

        return closest;
    }


    private void OnEnemyDead()
    {
        if (health <= 0)
        {
            isDead = true;
            EnemyManager.instance.UnregisterEnemy(this);
            gameObject.tag = "DeadEnemy";
            animator.enabled = false;

            if(agent != null)
            {
                agent.enabled = false;
            }

            if(obstacle != null)
            {
                obstacle.enabled = true;
            }

            EnableRagdoll();
        }
    }

    #endregion

    #region Attack Sequence & Behaviour

    
    IEnumerator BehaviorLoop()
    {
        while(true)
        {

            switch (currentState)
            {
                case State.Approaching:
                    yield return StartCoroutine(Approach());
                    break;
                case State.Attacking:
                    yield return StartCoroutine(Attack());
                    break;
            }

            yield return null;
        }
    }

    IEnumerator Approach()
    {
        if (isDead) yield break;
        
        Debug.Log($"{currentState}");

        agent.isStopped = false;
        animator.SetBool("Approaching", true);
       
        while (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            agent.SetDestination(player.position);
            yield return null;
        }

        currentState = State.Attacking;
    }

    IEnumerator Attack()
    {
        if (isDead) yield break;
       
        if(Time.time - lastAttackTime < attackCooldown)
        {

            yield return null;
            currentState = State.Approaching;
            yield break;

        }
        lastAttackTime = Time.time;

        ResetAnimationBools();
        transform.LookAt(player);
        Debug.Log($"{currentState}");
        animator.SetBool("Attacking", true);

        yield return new WaitForSeconds(1f);
        
        currentState = State.Approaching;
    }

    private void ResetAnimationBools()
    {
        animator.SetBool("Approaching", false);
        animator.SetBool("Attacking", false);
    }

    #endregion

    #region Animation Events

    [SerializeField] private GameObject rightHandCollider;
    [SerializeField] private GameObject leftHandCollider;

    void EnableColliderRH()
    {
        rightHandCollider.SetActive(true);
    }
    void DisableColliderRH()
    {
        rightHandCollider.SetActive(false);
    }

    void EnableColliderLH()
    {
        leftHandCollider.SetActive(true);
    }
    void DisableColliderLH()
    {
        leftHandCollider.SetActive(false);
    }

    #endregion
}