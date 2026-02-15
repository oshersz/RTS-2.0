using UnityEngine;

public class AOESkill : Spells
{
    //[HideInInspector] public new Transform target; //not sure what this does
    public float timeBeforeActive;
    public float timeAfterActive;
    private BoxCollider BC;
    //[SerializeField] float destroyAfter;
    void Start()
    {
        BC = GetComponent<BoxCollider>();

        Destroy(gameObject, timeAfterActive+timeBeforeActive);

        timeAfterActive += timeBeforeActive + Time.time;
        timeBeforeActive += Time.time;
        attackType = Attacks.Attack.AreaOfEffect;
    }

    void Update()
    {
        if (BC.enabled) //DON"T CHANGE THE ORDER
            BC.enabled = false;
        if (Time.time>timeBeforeActive)
        {
            BC.enabled = true;
            timeBeforeActive = float.MaxValue;
        }

    }
}
