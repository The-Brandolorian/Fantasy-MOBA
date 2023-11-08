using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite sprite;
    public new string name;
    public float cooldownTime = 10;
    public float castTime = 0.25f;
    public float range = 1;
    public bool hasAnimation = false;
    public bool stopsMovement = false;

    public enum AbilityType
    {
        Basic,
        Skillshot,
        Cone,
        AoE
    }
    public AbilityType type = AbilityType.Basic;
    public virtual KeyCode Key { get; }

    // The ability function
    public abstract void Activate(GameObject parent, RaycastHit hit);

    // Could be implemented to run while on cooldown. (e.g. debuffs)
    public virtual void Cooldown(GameObject parent) { }
}
