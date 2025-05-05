using Unity.VisualScripting;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endGameTrigger;
    [SerializeField] private GameManager endGameManager;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private bool playerInTrigger = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {  
            endGameUI.SetActive(true);
            playerInTrigger = true;
        }
    }

    public void Update()
    {
        if(playerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            endGameManager.OnGameOver.Invoke();
            endGameUI.SetActive(false);
        }
         
    }
   

}


