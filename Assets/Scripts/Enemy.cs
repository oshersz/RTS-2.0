using UnityEngine;

public class Enemy : Creature
{
    private Collider[] hits;

    private Collider[] charDetect;

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
        charDetect = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        if (charDetect.Length>0)
            Move(charDetect[0].transform);
        else
        {
            currentDespawnTime -= Time.deltaTime; // consider changing since they might still despawn while on the player's screen
            if (currentDespawnTime <= 0)
                Destroy(gameObject);
            Move();
        }

        

        hits = Physics.OverlapSphere(transform.position, 1,hitLayer);

        if (hits.Length>0)
        {
            for (int i = 0;i<hits.Length;i++)
            {
                Attacks currentAttack = hits[i].GetComponent<Attacks>(); 
                if (currentAttack.attackType == Attack.Targeted && currentAttack.target == transform) // && targeted
                {
                    Destroy(hits[i].gameObject);
                    TakeDamage(currentAttack.damage);
                    //ChangeBehavior();
                }
                else if (currentAttack.attackType == Attack.FirstTarget)
                {
                    Destroy(hits[i].gameObject);
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
    /*
    void ChangeBehavior()
    {
        if (behaviorType == BehaviorType.Neutral)
        {
            behaviorType = BehaviorType.Agressive;
            movementType = MovementType.Chasing;
        }
        if (behaviorType == BehaviorType.FrightenedOnAttack)
            movementType = MovementType.Running;

        if (detectionType == DetectionType.OnLineOfSight)
            detectionType = DetectionType.OnRadius;
    }
    */

}
