using UnityEngine;

public class SpawnRoomTrigger : MonoBehaviour
{
    [SerializeField] private SpawnRoom roomToDeactivate;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
            if(spawner != null )
            {
                spawner.DeactivateRoom(roomToDeactivate);
                Debug.Log("Deactivated spawn room: " + roomToDeactivate.name); 
                
                Destroy(gameObject);
            }
        }

       
    }
}
