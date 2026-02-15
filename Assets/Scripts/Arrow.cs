using UnityEngine;

public class Arrow : Attacks
{
    [SerializeField] float flySpeed;
    [SerializeField] float destroyAfter;
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
}
