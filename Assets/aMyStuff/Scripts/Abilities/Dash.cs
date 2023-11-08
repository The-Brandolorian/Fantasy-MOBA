using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu]
public class Dash : Ability
{
    [Header("Skill Projectile Variables")]
    public float returnTimeLimit = 5;

    private Vector3 position;
    private Vector3 originalPosition;
    private float timeLimit;
    private float timePassed;
    private GameObject parent;

    public override void Activate(GameObject parent, RaycastHit hit)
    {
        originalPosition = parent.transform.position;
        position = hit.point;
        this.parent = parent;

        if (hit.collider.tag == "Ground") parent.transform.position = hit.point; // only these two line matters, attempted to setup a return but did not get it.
        if (hit.collider.tag == "Enemy")
        {
            parent.transform.position = hit.point;
            Stats hitStats = hit.collider.gameObject.GetComponent<Stats>();
            Stats parentStats = parent.GetComponent<Stats>();

            if (hitStats && parentStats)
            {
                hitStats.TakeDamage(parentStats.attackDamage);
            }
        }
        //MonoInstance.instance.StartCoroutine(WaitForInput());
    }

    public IEnumerator WaitForInput()
    {
        timePassed = Time.time;
        timeLimit = Time.time + returnTimeLimit;

        while (timePassed < timeLimit)
        {
            Debug.Log("In while : " + timePassed + " - " + timeLimit);
            timePassed = timePassed + Time.deltaTime;
            if (Input.GetKeyDown(this.Key)) parent.transform.position = originalPosition;
        }

        yield return new WaitForSeconds(1);
    }
}
