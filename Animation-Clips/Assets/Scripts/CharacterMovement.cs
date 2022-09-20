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
    AnimationController animationManager;

    public enum Animations
    {
        WALK,
        RUN,
        JUMP,
        IDLE
    }

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
        animationManager = GetComponent<AnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.triggered)
        {
            StartCoroutine(Jump());
        }
    
        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        float yaw = moveDir.x;
        Vector3 rotation = transform.eulerAngles;
        rotation.y += yaw;
        transform.eulerAngles = rotation;

        bool sprint = sprintAction.IsPressed();
        bool movement = Mathf.Abs(moveDir.y) > Mathf.Epsilon;

        body.velocity = transform.rotation * Vector3.forward * moveDir.y * (sprint ? sprintSpeed : moveSpeed);

        if (movement)
        {
            if (sprint) animationManager.Change(Animations.RUN);

            else animationManager.Change(Animations.WALK);
        }
        else
        {
            animationManager.Change(Animations.IDLE);
        }
    }

    IEnumerator Jump()
    {
        animationManager.Change(Animations.JUMP);

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

        yield return null;
    }
}
