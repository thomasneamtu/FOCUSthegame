using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private float shakeForce = 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void CamShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(shakeForce);
    }
}
