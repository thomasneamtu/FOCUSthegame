using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float acceleration = 5f;
    public float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    public bool isGrounded = true;
    private Vector3 currentVelocity;


    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        Vector3 smoothInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 movement = rawInput.normalized;

        controller.Move(movement * moveSpeed * Time.deltaTime);

        animator.SetFloat("InputHorizontal", smoothInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat("InputVertical", smoothInput.z, 0.1f, Time.deltaTime);  
        animator.SetFloat("Speed", smoothInput.magnitude); 

        if (rawInput.magnitude >= 0.1f)
        {
            Vector3 moveDir = cam.forward * rawInput.z + cam.right * rawInput.x;
            moveDir.y = 0f;
            moveDir.Normalize();

            currentVelocity = Vector3.Lerp(currentVelocity, moveDir * moveSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate to stop
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        controller.Move(currentVelocity * Time.deltaTime);

        if(!controller.isGrounded)
        {
            currentVelocity.y += gravity * Time.deltaTime;
        }
        else
        {
            currentVelocity.y = -5f;
        }

    }

    public static void RotateToward(Vector3 targetPos, Transform self, float speed = 10f)
    {
        Vector3 direction = (targetPos - self.position).normalized;
        direction.y = 0;

        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            self.rotation = Quaternion.Slerp(self.rotation, lookRotation, Time.deltaTime * speed);
        }
    }


}
