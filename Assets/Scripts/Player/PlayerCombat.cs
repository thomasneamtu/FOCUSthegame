using UnityEngine.Events;
using UnityEngine;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform targetEnemy;
    [SerializeField] private GameManager deathEvent;
    [SerializeField] private Transform endDoor;

    [Header("Health Settings")]
    public int health = 5;
    [SerializeField] private GameObject healthUIEffect;

    [Header("ComboInfo")]
    [SerializeField] private int hCombo = 0;
    [SerializeField] float comboResetTimer = 1.0f;
    [SerializeField] float hLast;
    [SerializeField] private int jCombo = 0;
    [SerializeField] private float jLast;
    private float targetRefresh = 0.1f;
    private float targetUpdate = 0.1f;


    /*
    [Header("SphereCast")]
    [SerializeField] public LayerMask targetLayer;
    [SerializeField] private float checkArea = 1f;
    [SerializeField] private float punchForce = 5f;
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private Transform rFistCast;
    [SerializeField] private Transform lFistCast;
    [SerializeField] private Transform rFootCast;
    [SerializeField] private Transform lFootCast;
    */

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        targetEnemy = FindClosestEnemy();

        if(targetEnemy != null )
        {
            PlayerMovement.RotateToward(targetEnemy.position, transform);
        }
        else
        {
            PlayerMovement.RotateToward(endDoor.position, transform);
        }

       

        ClosestTargetUpdate();

        HandleCombo("h", ref hCombo, ref hLast);
        HandleCombo("j", ref jCombo, ref jLast);
    }

    /*
    public void AttackCast(Vector3 punchDirection) //WIP
    {
        Vector3 rfistBeam = rFistCast.position;
        Vector3 lfistBeam = lFistCast.position;
        Vector3 rfootBeam = rFootCast.position;
        Vector3 lfootBeam = lFootCast.position;

        RaycastHit hit;

        if (Physics.SphereCast(rfistBeam, checkArea, punchDirection, out hit, targetLayer))
        {
            Rigidbody hitRigidbody = hit.rigidbody;
            
            if(hitRigidbody != null)
            {
                Debug.Log("RigidBody Hit!");
                hitRigidbody.AddForce(punchDirection * punchForce, ForceMode.Impulse);
            }
        }

        if (Physics.SphereCast(lfistBeam, checkArea, punchDirection, out hit, targetLayer))
        {
            Rigidbody hitRigidbody = hit.rigidbody;

            if(hitRigidbody != null)
            {
                Debug.Log("RigidBody Hit!");
                hitRigidbody.AddForce(punchDirection * punchForce, ForceMode.Impulse);
            }
        }

        if(Physics.SphereCast(rfootBeam, checkArea, punchDirection, out hit, targetLayer))
        {
            Rigidbody hitRigidbody = hit.rigidbody;

            if(hitRigidbody != null)
            {
                Debug.Log("RigidBody Hit!");
                hitRigidbody.AddForce(punchDirection * kickForce, ForceMode.Impulse);
            }
        }

        if(Physics.SphereCast(lfootBeam, checkArea, punchDirection, out hit, targetLayer))
        {
            Rigidbody hitRigidbody = hit.rigidbody;

            if(hitRigidbody != null)
            {
                Debug.Log("RigidBody Hit!");
                hitRigidbody.AddForce(punchDirection * kickForce, ForceMode.Impulse);
            }
        }

    }
    */

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("EnemyHD"))
        {
            int damage = 1;
            TakeDamage(damage);

            animator.SetTrigger("Hit");
            //camerashake, add dazed effect !!
        }
    }


    public void TakeDamage(int amount)
    {
        health -= amount;

        if(health <= 2)
        {
            healthUIEffect.SetActive(true);
        }
        else
        {
            healthUIEffect.SetActive(false);
        }

        if (health <= 0)
        {
            OnPlayerDeath();
        }
    }
        
    public void OnPlayerDeath()
    {
        deathEvent.OnPlayerDead.Invoke();
    }

    #region Combat
    private void ClosestTargetUpdate()
    {
        if(Time.time >= targetUpdate)
        {
            targetUpdate = Time.time + targetRefresh;

            Transform newTarget = FindClosestEnemy();

            if (newTarget != null && newTarget != targetEnemy)
            {
                targetEnemy = newTarget;
            }

           
        }
    }
    private void HandleCombo(string key, ref int Combo, ref float Last)
    {
        if(Input.GetKeyDown(key))
        {
            if (Time.time - Last > comboResetTimer)
            {
                Combo = 0;
            }
           
            Combo++;
            Last = Time.time;

            if (Combo > 4) Combo = 1;

            string triggerName = key.ToUpper() + Combo;

            ResetComboTriggers();

            animator.SetTrigger(triggerName);

            Debug.Log("Triggering: " + triggerName);
            
        }
    }
    Transform FindClosestEnemy(float maxLockOnDistance = 10f, float maxLockOnAngle = 90f)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float closestDistance = maxLockOnDistance;

        foreach(var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            if (dist < closestDistance)
            {
                Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, toEnemy);

                if(angle < maxLockOnAngle)
                {
                    closestEnemy = enemy.transform;
                    closestDistance = dist;
                }
            }
        }
         return closestEnemy;
    }
    public void ResetComboTriggers()
    {
        string[] triggers = { "H1", "H2", "H3", "H4", "J1", "J2", "J3", "J4" };
        foreach (var t in triggers)
        {
            animator.ResetTrigger(t);
        }
    }
    #endregion

    #region Animation Events
    [Header("Colliders")]
    [SerializeField] private GameObject rightFootCollider;
    [SerializeField] private GameObject leftFootCollider;
    [SerializeField] private GameObject leftHandCollider;
    [SerializeField] private GameObject rightHandCollider;

    void EnableColliderRF()
    {
        rightFootCollider.SetActive(true);
    }

    void DisableColliderRF()
    {
        rightFootCollider.SetActive(false);
    }

    void EnableColliderLF()
    {
        leftFootCollider.SetActive(true);
    }

    void DisableColliderLF()
    {
        leftFootCollider.SetActive(false);
    }

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