using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction jumpAction;
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float sprintSpeed = 4;
    [SerializeField] float jumpPower = 5;
    [SerializeField] float jumpDuration = 0.5f;

    Rigidbody body;
    Animator animator;

    float speed;

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.triggered)
        {
            StartCoroutine(Jump());
        }

        if (sprintAction.IsPressed())
        {
            speed = sprintSpeed;
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
            speed = moveSpeed;
        }

        

        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveDir.x, 0, moveDir.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        body.velocity = moveDirection * speed;
    }

    IEnumerator Jump()
    {
        animator.SetBool("Jump", true);

        float elaspedTime = 0;

        while(elaspedTime < jumpDuration / 2)
        {
            body.velocity = new Vector3(body.velocity.x, Mathf.Lerp(jumpPower, 0, elaspedTime / (jumpDuration / 2)), body.velocity.z);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        elaspedTime = 0;

        while (elaspedTime < jumpDuration / 2)
        {
            body.velocity = new Vector3(body.velocity.x, Mathf.Lerp(0, -jumpPower, elaspedTime / (jumpDuration / 2)), body.velocity.z);

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        body.velocity = Vector3.zero;

        animator.SetBool("Jump", false);

        yield return null;
    }
}
