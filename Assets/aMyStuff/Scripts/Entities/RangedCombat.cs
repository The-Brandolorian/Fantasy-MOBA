using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedCombat : Combat
{
    [Header("Ranged Projectile Variables")]
    public GameObject attackProjectile;
    public Transform projectileSpawnPoint;
    private GameObject spawnedProjectile;

    protected override void Attack()
    {
        if (target && Vector3.Distance(target.transform.position, transform.position) < stats.attackRange)
        {
            spawnedProjectile = Instantiate(attackProjectile, projectileSpawnPoint.transform.position, Quaternion.identity);

            ProjectileTarget projectileTarget = spawnedProjectile.GetComponent<ProjectileTarget>();
            if (projectileTarget) projectileTarget.SetTarget(target.transform, stats.attackDamage);

            nextAttackTime = Time.time + attackInterval;
        }

        if (animator) animator.SetBool("isAttacking", false);
        performAttack = true;
    }
}
