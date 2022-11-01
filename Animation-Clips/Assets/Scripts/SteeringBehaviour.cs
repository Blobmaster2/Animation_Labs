using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float arriveSlowdownThreshold = 5;

    bool enrouteToRandomPos = false;
    public Transform wanderTargetPos;

    Rigidbody rb;
    [SerializeField] SteeringBehaviours behaviours;

    enum SteeringBehaviours
    {
        SEEK,
        FLEE,
        ARRIVE,
        WANDER
    };

    private void Start()
    {
        StartCoroutine(WaitForNewPosition());

        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        ApplyBehaviour();
       
    }

    
    void ApplyBehaviour()
    {
        switch (behaviours)
        {
            case SteeringBehaviours.SEEK:
                Seek(false);
                break;
            case SteeringBehaviours.FLEE:
                Seek(true);
                break;
            case SteeringBehaviours.WANDER:
                Wander();
                break;
            case SteeringBehaviours.ARRIVE:
                Arrive();
                break;
            default:
                throw new System.Exception("Something went wrong...");
        }
    }

    void Seek(bool switchToFlee)
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = targetDirection * speed;
        Vector3 desiredVelocity = targetVelocity - currentVelocity;

        if (Vector3.Distance(transform.position, target.position) > 10 && switchToFlee) desiredVelocity = Vector3.zero;

        gameObject.transform.LookAt(target);

        if (switchToFlee) rb.AddForce(-desiredVelocity / speed);
        else rb.AddForce(desiredVelocity);

    }

    void Wander()
    {
        if (enrouteToRandomPos)
        {
            Vector3 targetDirection = (wanderTargetPos.position - transform.position).normalized;
            Vector3 currentVelocity = rb.velocity;
            Vector3 targetVelocity = targetDirection * speed;
            Vector3 desiredVelocity = targetVelocity - currentVelocity;

            gameObject.transform.LookAt(wanderTargetPos);

            rb.AddForce(desiredVelocity);
        }

        if (Vector3.Distance(transform.position, wanderTargetPos.position) < 1.0f)
        {
            rb.angularVelocity = Vector3.zero;
            StartCoroutine(WaitForNewPosition());
            enrouteToRandomPos = false;

        }
    }

    void Arrive()
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 currentVelocity = rb.velocity;
        float targetSpeed = speed * (Vector3.Distance(transform.position, target.position) / arriveSlowdownThreshold);

        if (targetSpeed > speed) targetSpeed = speed;

        Vector3 targetVelocity = targetDirection * targetSpeed;
        Vector3 desiredVelocity = targetVelocity - currentVelocity;

        gameObject.transform.LookAt(target);

        rb.AddForce(desiredVelocity);
    }

    Vector3 CreateRandomPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10, 10), Random.Range(1, 10), 0);
        return randomPos;
    }

    IEnumerator WaitForNewPosition()
    {
        wanderTargetPos.position = CreateRandomPosition();

        yield return new WaitForSeconds(Random.Range(0, 2));

        enrouteToRandomPos = true;
    }
}
