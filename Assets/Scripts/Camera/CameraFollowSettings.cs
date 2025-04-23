using UnityEngine;

public class CameraFollowSettings : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fixedY;
    [SerializeField] private float fixedZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 newPos = transform.position;
        newPos.x = player.position.x;
        newPos.y = fixedY;
        newPos.z = fixedZ;
        transform.position = newPos;
    }
}
