using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] Animator characterAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           // characterAnimator.SetTrigger("JAB");
        }
    }
}
