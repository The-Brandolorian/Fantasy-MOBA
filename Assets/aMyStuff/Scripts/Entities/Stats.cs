using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Health Settings")] 
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject floatingHealthBarPrefab;
    [SerializeField] private Vector3 floatingHealthBarPosition = new Vector3(0,2.7f,0);
    [SerializeField] private Quaternion floatingHealthBarRotation = new Quaternion(0.5f, 0, 0, 0.866025388f);
    [SerializeField] private float scale = 1f;

    [Header("Attack Settings")]
    [SerializeField] private int baseAttackDamage = 10;
    [SerializeField] private float baseAttackRange = 5;
    [SerializeField] private float baseAttackSpeed = 3;

    [Header("Respawn Settings")]
    [SerializeField] private bool doRespawn = false;
    [SerializeField] private Vector3 respawnPos;
    [SerializeField] private float respawnTime = 5;
    private bool isDying = false;

    [Header("Current Values")]
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;

    private FillBar healthBar;
    private float health;

    private Animator animator;
    private Movement movement;

    // runs before Start().
    void Awake()
    {
        health = maxHealth;
        attackRange = baseAttackRange;
        attackDamage = baseAttackDamage;
        attackSpeed = baseAttackSpeed;

        animator = GetComponent<Animator>();
        if (gameObject.tag == "Player") movement = GetComponent<PlayerMovement>();
        else movement = GetComponent<AI_Movement>();

        // If no repsawn position set, assign current location to it.
        if (doRespawn)
        {
            if (respawnPos == Vector3.zero) respawnPos = transform.position;
        }

        // Create floating health bar object from prefab, not having it hard coded allows us to spawn any entity with a health bar.
        GameObject newBar = Instantiate(floatingHealthBarPrefab, floatingHealthBarPosition, floatingHealthBarRotation);
        newBar.name = (gameObject.name + " Health Bar");
        newBar.transform.localScale = new Vector3(newBar.transform.localScale.x * scale, newBar.transform.localScale.y * scale, newBar.transform.localScale.z * scale);

        // Resets the position so it can be relative to the parent entity.
        newBar.transform.position = floatingHealthBarPosition;

        // Sets the bars parent to the parent of this stats instance, allowing it to follow the entity.
        newBar.transform.SetParent(gameObject.transform, false);
        healthBar = newBar.GetComponent<FillBar>();
    }

    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // If a fill bar is present, set fill percentage.
        if (healthBar) healthBar.SetFillAmount(health / maxHealth); 

        // If health diminished, kill entity.
        if (health <= 0 && !isDying) StartCoroutine(DoDeathProcess());
    }

    private IEnumerator DoDeathProcess()
    {
        isDying = true;

        if (animator) animator.SetBool("isDying", true);
        else Death();

        // Wait based on attack speed.
        yield return new WaitForSeconds(respawnTime);
    }

    public void Death()
    {
        if (doRespawn)
        {
            movement.StopMovement();
            gameObject.transform.position = respawnPos;
            if (animator) animator.SetBool("isDying", false);
            health = maxHealth;
            healthBar.SetFillAmount(1);
            isDying = false;
        }

        else Destroy(gameObject);
    }
}
