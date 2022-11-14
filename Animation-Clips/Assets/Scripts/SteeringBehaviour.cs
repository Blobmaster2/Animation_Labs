using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float arriveSlowdownThreshold = 5;

    public int pursuitGameUpdates = 1;

    public float customProximity = 5;

    bool enrouteToRandomPos = false;
    public Transform wanderTargetPos;

    Rigidbody rb;
    public SteeringBehaviours behaviours;
    public CustomBehaviours customBehaviours;

    public bool useCustomBehaviour;

    public enum SteeringBehaviours
    {
        SEEK,
        FLEE,
        ARRIVE,
        PROX_FLEE,
        PURSUE,
        EVADE,
        WANDER
    };

    public enum CustomBehaviours
    {
        FLEE_SEEK,
        WANDER_PROXFLEE,
        REVERSE_ARRIVE,
        WANDER_PROXSEEK
    };

    private void Start()
    {
        StartCoroutine(WaitForNewPosition());

        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        if (useCustomBehaviour) ApplyCustomBehaviours();
        else ApplyBehaviour();

        //speed control ---> (if velocity > max speed, add negative velocity)
        if (rb.velocity.magnitude > speed)
        {
            //rb.AddForce(-rb.velocity);
        }
    }

    void ApplyCustomBehaviours()
    {
        float t = 1f - Attenuate(target.position, transform.position, customProximity);
        Vector3 desiredVelocity = new();

        switch (customBehaviours)
        {
            case CustomBehaviours.FLEE_SEEK:
                {
                    Vector3 a = Seek();
                    Vector3 b = -Seek();
                    desiredVelocity = Vector3.Lerp(a, b, t);
                }

                break;

            case CustomBehaviours.WANDER_PROXFLEE:
                {
                    Vector3 a = Wander();
                    Vector3 b = Vector3.Distance(target.position, transform.position) < 10 ? -Seek() : Vector3.zero;
                    desiredVelocity = Vector3.Lerp(a, b, t);
                }

                break;

            case CustomBehaviours.REVERSE_ARRIVE:
                {
                    Vector3 a = Arrive();
                    Vector3 b = rb.velocity;
                    desiredVelocity = Vector3.Lerp(a, b, t);
                }

                break;

            case CustomBehaviours.WANDER_PROXSEEK:
                {
                    Vector3 a = Wander();
                    Vector3 b = Vector3.Distance(target.position, transform.position) < 10 ? Seek() : Vector3.zero;
                    desiredVelocity = Vector3.Lerp(a, b, t);
                }

                break;
        }

        rb.AddForce(desiredVelocity);

    }

    void ApplyBehaviour()
    {
        switch (behaviours)
        {
            case SteeringBehaviours.SEEK:
                rb.AddForce(Seek());
                break;

            case SteeringBehaviours.FLEE:
                rb.AddForce(-Seek());
                break;

            case SteeringBehaviours.PROX_FLEE:
                if (Vector3.Distance(transform.position, target.transform.position) < 10)
                {
                    rb.AddForce(-Seek());
                }
                else
                {
                    rb.AddForce(-rb.velocity);
                }
                break;

            case SteeringBehaviours.WANDER:
                rb.AddForce(Wander());
                break;

            case SteeringBehaviours.PURSUE:
                rb.AddForce(Pursue());
                break;

            case SteeringBehaviours.EVADE:
                if (Vector3.Distance(transform.position, target.transform.position) < 10)
                {
                    rb.AddForce(-Pursue());
                }
                else
                {
                    rb.AddForce(-rb.velocity);
                }
                break;

            case SteeringBehaviours.ARRIVE:
                rb.AddForce(Arrive());
                break;

            default:
                throw new System.Exception("Something went wrong...");
        }
    }

    Vector3 Seek()
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = targetDirection * speed;
        Vector3 desiredVelocity = targetVelocity - currentVelocity;

        gameObject.transform.LookAt(target);

        return desiredVelocity;
    }

    Vector3 Wander()
    {
        Vector3 desiredVelocity = new Vector3();

        if (enrouteToRandomPos)
        {
            Vector3 targetDirection = (wanderTargetPos.position - transform.position).normalized;
            Vector3 currentVelocity = rb.velocity;
            Vector3 targetVelocity = targetDirection * speed;
            desiredVelocity = targetVelocity - currentVelocity;

            gameObject.transform.LookAt(wanderTargetPos);
        }

        if (Vector3.Distance(transform.position, wanderTargetPos.position) < 1.0f && enrouteToRandomPos)
        {
            rb.angularVelocity = Vector3.zero;
            StartCoroutine(WaitForNewPosition());
            enrouteToRandomPos = false;
        }

        return desiredVelocity;
    }

    Vector3 Arrive()
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;
        Vector3 currentVelocity = rb.velocity;
        float targetSpeed = speed * (Vector3.Distance(transform.position, target.position) / arriveSlowdownThreshold);

        if (targetSpeed > speed) targetSpeed = speed;

        Vector3 targetVelocity = targetDirection * targetSpeed;
        Vector3 desiredVelocity = targetVelocity - currentVelocity;

        gameObject.transform.LookAt(target);

        return desiredVelocity;
    }

    Vector3 Pursue()
    {
        Vector3 targetDirection = (target.position + target.GetComponent<Rigidbody>().velocity * pursuitGameUpdates - transform.position).normalized;
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = targetDirection * speed;
        Vector3 desiredVelocity = targetVelocity - currentVelocity;

        gameObject.transform.LookAt(target.position + target.GetComponent<Rigidbody>().velocity * pursuitGameUpdates);

        if (Vector3.Distance(transform.position, target.position) > 10) desiredVelocity = Vector3.zero;

        return desiredVelocity;
    }

    Vector3 CreateRandomPosition()
    {
        Vector3 randomPos = new(Random.Range(transform.position.x - 10, transform.position.x + 10), Random.Range(transform.position.x - 10, transform.position.x + 10), 0);
        return randomPos;
    }

    IEnumerator WaitForNewPosition()
    {
        wanderTargetPos.position = CreateRandomPosition();

        yield return new WaitForSeconds(Random.Range(0, 2));

        enrouteToRandomPos = true;
    }

    float Attenuate(Vector3 target, Vector3 current, float length)
    {
        return Mathf.Clamp01((target - current).magnitude / length);
    }
}
