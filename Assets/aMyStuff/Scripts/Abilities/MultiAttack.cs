using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MultiAttack : Ability
{
    [Header("Skill Projectile Variables")]
    public GameObject attackProjectile;
    public Transform projectileSpawnPoint;
    public int damage;
    public float fireRate;

    private GameObject parent;
    private Vector3 direction;
    private bool isCasting;

    public override void Activate(GameObject parent, RaycastHit hit)
    {
        direction = ((hit.point - parent.transform.position) + new Vector3(0, 1, 0)).normalized;
        this.parent = parent;

        // Attempt to set spawn point otherwise just use parents tranform.
        projectileSpawnPoint = parent.transform.Find("Projectile Spawn Point");
        if (projectileSpawnPoint == null) projectileSpawnPoint = parent.transform;

        if (!isCasting) MonoInstance.instance.StartCoroutine(FireProjectiles());
    }

    private IEnumerator FireProjectiles()
    {
        isCasting = true;

        // Fire x amount of projectiles.
        float randValue = Random.Range(2, 5);
        for (int i = 0; i < randValue; i++)
        {
            GameObject projectile = GameObject.Instantiate(attackProjectile, projectileSpawnPoint.position, Quaternion.identity);

            ProjectileTarget projectileTarget = projectile.GetComponent<ProjectileTarget>();
            if (projectileTarget)
            {
                projectileTarget.SetDirection(direction, parent.transform.position, damage, range);
            }

            // Wait based on attack speed.
            yield return new WaitForSeconds(fireRate);
        }
        isCasting = false;
    }
}
