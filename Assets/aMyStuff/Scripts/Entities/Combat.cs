using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stats))]
public class Combat : MonoBehaviour
{
    protected Stats stats;
    protected Animator animator;
    protected Movement movement;

    [Header("Target")]
    public GameObject target;
    
    [Header("Attack Variables")]
    public bool performAttack = true;
    protected float attackInterval;
    protected float nextAttackTime = 0;

    void Start()
    {
        stats = GetComponent<Stats>();
        animator = GetComponent<Animator>();
        if (gameObject.tag == "Player") movement = GetComponent<PlayerMovement>();
        else movement = GetComponent<AI_Movement>();
    }

    void Update()
    {
        // Calculates the attack speed and interval between each attack.
        attackInterval = stats.attackSpeed / ((500 + stats.attackSpeed) * 0.01f);
        if (movement?.currentTarget) target = movement.currentTarget;

        // Check if null, can attack and distance is less than attack range, if yes perform attack.
        if (target != null && performAttack && Time.time > nextAttackTime)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= stats.attackRange)
            {
                // Check target is not doing death animation if they have animator.
                bool alive = true;
                Animator anim = target.gameObject.GetComponent<Animator>();
                if (anim) alive = !anim.GetBool("isDying");
                
                if (alive) StartCoroutine(DoAttackInterval());
            }
        }
    }

    protected IEnumerator DoAttackInterval()
    {
        performAttack = false;
        // Trigger animation.
        if (target != null)
        {
            // If using animation to trigger attack enable it, if not just call attack function.
            if (animator) animator.SetBool("isAttacking", true);
            else Attack();

            // Wait based on attack speed.
            yield return new WaitForSeconds(attackInterval);
        }

        // Reset values on enemy death or target clear.
        else
        {
            if (animator) animator.SetBool("isAttacking", false);
            performAttack = true;
        }
    }

    protected virtual void Attack()
    {
        Stats targetStats = target.gameObject.GetComponent<Stats>();
        targetStats?.TakeDamage(stats.attackDamage);

        nextAttackTime = Time.time + attackInterval;
        performAttack = true;
        if (animator) animator.SetBool("isAttacking", false);
    }
}
