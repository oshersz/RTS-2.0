using UnityEngine;

public class Arrow : Attacks
{
    [SerializeField] float flySpeed;
    [SerializeField] float destroyAfter;
    [SerializeField] GameObject effect;

    private float destroyTime;
    void Start()
    {
        attackType = Attack.Targeted;
        //attackType = Attack.FirstTarget;
        Destroy(gameObject, destroyAfter); //0.5f
    }

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    public override void DestroyAttack()
    {
        if (effect!=null)
        {
            effect.transform.parent = null;
            Destroy(effect.gameObject, (destroyAfter));
        }

        Destroy(gameObject);
    }
}
