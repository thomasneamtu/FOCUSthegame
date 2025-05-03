
using UnityEngine;

public class DeathRagdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] ragdolls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        ragdolls = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisableRagdoll()
    {
        foreach(var rigidbody in ragdolls)
        {
            rigidbody.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        foreach(var rigidbody in ragdolls)
        {
            rigidbody.isKinematic = false;
        }
    }
}
