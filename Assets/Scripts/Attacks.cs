using UnityEngine;

public abstract class Attacks : MonoBehaviour
{
    public Attack attackType;
    public float damage;
    public Transform target;
    public enum Attack
    {
        FirstTarget,
        Targeted,
        AreaOfEffect,
        DamageOverTime
    }
}
