using UnityEngine;

public class Enemy : Creature
{
    private Collider[] hits;

    private Collider[] charDetect;

    private Collider[] creatureDetect;

    private float closestDistance = float.MaxValue;
    private Transform closestCreature;

    void Start()
    {
        startingPosition = transform.position;

        int behaviourChangeChance = Random.Range(0,5);
        if (behaviourChangeChance == 3)
        {
            movementType = MovementType.LinearPatroling;
        }
        if (behaviourChangeChance == 4)
        {
            movementType = MovementType.CircularPatroling;
        }
    }

    void Update()
    {
        if (UIManager.framer%5==0)
        {
            DetectCreatures();
        }

        if (UIManager.framer%20==0)
        {
            

            charDetect = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

            if (charDetect.Length > 0)
            {
                for (int i = 0; i < charDetect.Length; i++)
                {
                    float distance = Vector3.Distance(charDetect[i].transform.position, transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCreature = charDetect[i].transform;
                    }
                }
            }
            else
            {
                currentDespawnTime -= Time.deltaTime; // consider changing since they might still despawn while on the player's screen
                if (currentDespawnTime <= 0)
                    Destroy(gameObject);
            }
        }

        if (closestCreature==null)
        {
            Move();
        }
        else
        {
            Move(closestCreature);
        }
        
        DetectHits();
    }

    public void DetectHits()
    {
        hits = Physics.OverlapSphere(transform.position, 1, hitLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Attacks currentAttack = hits[i].GetComponent<Attacks>();
                if (currentAttack.attackType == Attack.Targeted && currentAttack.target == transform) // && targeted
                {
                    //Destroy(hits[i].gameObject);
                    currentAttack.DestroyAttack();
                    TakeDamage(currentAttack.damage);
                    //ChangeBehavior();
                }
                else if (currentAttack.attackType == Attack.FirstTarget)
                {
                    //Destroy(hits[i].gameObject);
                    currentAttack.DestroyAttack();
                    TakeDamage(currentAttack.damage);
                    //ChangeBehavior();
                }
                else if (currentAttack.attackType == Attack.AreaOfEffect)
                {
                    TakeDamage(currentAttack.damage);
                    //ChangeBehavior();
                }
            }
        }
    }


    private void DetectCreatures()
    {
        LayerMask creatureMask = LayerMask.GetMask("Ally","Enemy");
        creatureDetect = Physics.OverlapSphere(transform.position, 1, creatureMask);

        if (creatureDetect.Length > 0)
        {
            if (creatureDetect.Length == 1 && creatureDetect[0].transform == transform)
            {
                return;
            }

            for (int i = 0; i < creatureDetect.Length; i++)
            {
                float distance = Vector3.Distance(creatureDetect[i].transform.position, transform.position);

                if (distance < 2)
                {
                    Vector3 moveDirection = (creatureDetect[i].transform.position - transform.position).normalized;
                    moveDirection.y = 0;
                    creatureDetect[i].transform.position += moveDirection * 0.03f * Mathf.Clamp(2f - distance, 0, 2); //0.0675    *0.02f
                }
            }

        }

    }

}
