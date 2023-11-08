using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Stats))]
public class Movement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public Stats stats;

    public float rotateSpeedMovement = 0.05f;
    public bool canMove = true;

    public float stoppingDistance = 1;
    public GameObject currentTarget;

    protected float rotateVelocity;
    protected float motionSmoothTime = 0.1f;

    // Runs before Start().
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<Stats>();
    }

    private void Update()
    {
        // Updates rotation if target still exists.
        if (currentTarget != null)
        {
            Rotation(currentTarget.transform.position);
        }
    }

    public void Animation()
    {
        // Set animation speed based on character speed
        float speed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speed, motionSmoothTime, Time.deltaTime);
    }

    public void StopMovement()
    {
        if (currentTarget != null) currentTarget = null;
        agent.isStopped = true;
    }

    public void MoveToPosition(Vector3 position)
    {
        // Set destination to vector, clearing target if there is one. Allowing you to cancel attacking by moving.
        if (currentTarget != null) currentTarget = null;
        agent.isStopped = false;
        agent.SetDestination(position);
        agent.stoppingDistance = 1;

        Rotation(position);
    }

    public void MoveToTarget(GameObject target)
    {
        // Follow target, combat script will get movement target and attempt to attack it.
        stoppingDistance = stats.attackRange;
        currentTarget = target;
        agent.isStopped = false;
        agent.SetDestination(currentTarget.transform.position);
        agent.stoppingDistance = stoppingDistance;

        Rotation(currentTarget.transform.position);
    }

    public void Rotation(Vector3 lookAtPosition)
    {
        // Sets roatation to look at target.
        Quaternion rotationToLookAt = Quaternion.LookRotation(lookAtPosition - transform.position);
        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationToLookAt.eulerAngles.y, ref rotateVelocity, rotateSpeedMovement * (Time.deltaTime * 5));
        transform.eulerAngles = new Vector3(0, rotationY, 0);
    }
}
