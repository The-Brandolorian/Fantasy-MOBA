using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : Movement
{
    private bool waitingForAttackToEnd = false;

    void Update()
    {
        Animation();
        Move();
    }

    public void Move()
    {
        // Get click position on right click, if we are already attacking then set bool to auto trigger attack/move after animation is over.
        // Then detect whether we clicked on the ground of an enemy.
        if (Input.GetMouseButtonDown(1) && canMove || waitingForAttackToEnd && canMove) 
        {
            if ((animator.GetBool("isAttacking") || animator.GetBool("isCasting")) != true)
            {
                waitingForAttackToEnd = false;

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Ground")) MoveToPosition(hit.point);
                    if (hit.collider.CompareTag("Enemy")) MoveToTarget(hit.collider.gameObject);
                }
            }
            else waitingForAttackToEnd = true;
        }

        // Set rotation to face target if exists.
        if (currentTarget != null)
        {
            Rotation(currentTarget.transform.position);
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > stoppingDistance) agent.SetDestination(currentTarget.transform.position);
        }
    }
}
