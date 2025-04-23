using UnityEngine.Events;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform targetEnemy;

    private float targetRefresh = 0.2f;
    private float targetUpdate = 0f;

    [Header("ComboInfo")]
    [SerializeField] private int hCombo = 0;
    [SerializeField] float comboResetTimer = 1.0f;
    [SerializeField] float hLast;

    [SerializeField] private int jCombo = 0;
    [SerializeField] private float jLast;

    [SerializeField] private int kCombo = 0;
    [SerializeField] private float kLast;

    [SerializeField] private int lCombo = 0;
    [SerializeField] private float lLast;

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

        ClosestTargetUpdate();

        HandleCombo("h", ref hCombo, ref hLast);
        HandleCombo("j", ref jCombo, ref jLast);
        HandleCombo("k", ref kCombo, ref kLast);
        HandleCombo("l", ref lCombo, ref lLast);
    }

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
        string[] triggers = { "H1", "H2", "H3", "H4", "J1", "J2", "J3", "J4", "K1", "K2", "K3", "K4", "L1", "L2", "L3", "L4" };
        foreach (var t in triggers)
        {
            animator.ResetTrigger(t);
        }
    }
}