using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProjectileTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 position;
    public Vector3 parentPosition;
    public float projectileRange;
    public float projectileSpeed;
    public int projectileDamage;

    private Vector3 projectileDirection;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        if (target != null)
        {
            // Target position is at objects feet.
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }
        else if (position != null)
        {
            rb.velocity = projectileDirection * projectileSpeed;
            if (Vector3.Distance(parentPosition, transform.position) > projectileRange) Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetTarget(Transform newTarget, int damage)
    {
        target = newTarget;
        projectileDamage = damage;

    }

    public void SetDirection(Vector3 direction, Vector3 newParentPosition, int damage, float range)
    {
        parentPosition = newParentPosition;
        projectileDamage = damage;
        projectileRange = range;
        projectileDirection = direction;
    }

    // Called when projectile enters another collider.
    private void OnTriggerEnter(Collider other)
    {
        // Check we hit the intended target.
        if (target != null && ReferenceEquals(other.gameObject, target.gameObject))
        {
            Stats targetStats = other.gameObject.GetComponent<Stats>();
            targetStats?.TakeDamage(projectileDamage);
            Destroy(gameObject);
        }

        // If we are firing at a position (player ability), we need to check if we hit an enemy.
        else if (target == null && position != null && other.tag == "Enemy")
        {
            Debug.Log("Hit target: " + other.name);
            Stats targetStats = other.gameObject.GetComponent<Stats>();
            targetStats?.TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
    }
}
