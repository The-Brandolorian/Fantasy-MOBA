using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Detection : MonoBehaviour
{
    public bool doIndicator = false;
    private Stats stats;
    private Combat combat;
    private AI_Movement movement;
    private SphereCollider detectionCollider;
    private Canvas rangeIndicator;

    [SerializeField] private List<GameObject> targetsEnteredRange;
    [SerializeField] private bool hasTarget = false;

    private GameObject player;

    void Start()
    {
        combat = GetComponentInParent<Combat>();
        stats = GetComponentInParent<Stats>();
        movement = GetComponentInParent<AI_Movement>();

        detectionCollider = GetComponent<SphereCollider>();
        detectionCollider.radius = stats.attackRange;

        rangeIndicator = transform.Find("Range Indicator").GetComponent<Canvas>();
        rangeIndicator.GetComponentInChildren<Image>().rectTransform.sizeDelta = new Vector2(stats.attackRange * 2, stats.attackRange * 2);

        targetsEnteredRange = new List<GameObject>();
    }

    void Update()
    {
        if (combat)
        {
            // If we have no current target, attempt to get one from the targets list, clearing invalid entries as we go.
            if (!hasTarget)
            {
                if (targetsEnteredRange.Count > 0)
                {
                    GameObject target = targetsEnteredRange[0];
                    if (target != null && Vector3.Distance(transform.position, target.transform.position) <= (stats.attackRange * 0.55f))
                    {
                        // attack target and stop moving.
                        combat.target = target;
                        if (movement) movement.StopMovement();
                        hasTarget = true;
                    }
                    else targetsEnteredRange.RemoveAt(0);
                }

                // set target to enemyBase when no more targets in range.
                else
                {
                    if (movement)
                    {
                        combat.target = movement.enemyBase;
                        movement.MoveToTarget(movement.enemyBase);
                    }
                    else combat.target = null;
                }
            }

            // Remove target if no longer in range (range needs to be just above half so they don't get removed as they get added).
            else
            {
                if (combat.target != null && Vector3.Distance(transform.position, targetsEnteredRange[0].transform.position) > (stats.attackRange * 0.55f))
                {
                    targetsEnteredRange.RemoveAt(0);
                    hasTarget = false;
                }
            }
        }

        // Clear range indicator when player leaves.
        if (doIndicator)
        {
            if (player != null)
            {
                if (Vector3.Distance(transform.position, player.transform.position) > (stats.attackRange * 0.55f))
                {
                    ToggleRangeIndicator();
                    player = null;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Display range indicator if player entered.
        if (doIndicator)
        {
            if (other.tag == "Player")
            {
                player = other.gameObject;
                ToggleRangeIndicator();
            }
        }

        if (combat)
        {
            // Get team tag from parent when object enters range, check if object is an enemy. If it is add it to targets list.
            if (transform.parent.tag == "Enemy")
            {
                if (other.tag == "Ally" || other.tag == "Player")
                {
                    targetsEnteredRange.Add(other.gameObject);
                }
            }

            else if (transform.parent.tag == "Ally")
            {
                if (other.tag == "Enemy")
                {
                    targetsEnteredRange.Add(other.gameObject);
                }
            }
        }
    }

    public void ToggleRangeIndicator()
    {
        if (rangeIndicator.enabled) rangeIndicator.enabled = false;
        else rangeIndicator.enabled = true;
    }
}
