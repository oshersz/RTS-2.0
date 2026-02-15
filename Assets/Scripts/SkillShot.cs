using UnityEngine;

public class SkillShot : Spells
{
    //[HideInInspector]public new Transform target; //not sure what this does
    [SerializeField] float flySpeed;
    [SerializeField] float destroyAfter;
    void Start()
    {
        attackType = Attacks.Attack.FirstTarget;
        Destroy(gameObject, destroyAfter);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);
    }
}
