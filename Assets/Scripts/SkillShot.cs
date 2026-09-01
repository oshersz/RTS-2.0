using UnityEngine;

public class SkillShot : Spells
{
    //[HideInInspector]public new Transform target; //not sure what this does
    [SerializeField] float flySpeed;
    [SerializeField] float destroyAfter;
    [SerializeField] ParticleSystem effect;

    private float destroyTime;
    private bool destroyed = false;
    void Start()
    {
        attackType = Attack.FirstTarget;
        destroyTime = Time.time + destroyAfter;
        //Destroy(gameObject, destroyAfter);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        if (Time.time>destroyTime && !destroyed)
        {
            destroyed = true;
            effect.transform.parent = null;
            Destroy(effect.gameObject, 1f);
            Destroy(gameObject);
        }


    }

    public override void DestroyAttack()
    {
        effect.transform.parent = null;
        Destroy(effect.gameObject, (1-(destroyTime - Time.time)));
        Destroy(gameObject);
    }

}
