using UnityEngine;

public class Ally : Creature
{
    private Collider[] hits;

    private Collider[] charDetect;
    private Collider[] creatureDetect;
    //[HideInInspector]new float currentDespawnTime; //is this correct?

    private Transform closestPlayer;
    private Transform closestEnemy;

    bool allyAdded;

    private bool colliding;

    void Start()
    {
        startingPosition = transform.position;

        behaviorType = BehaviorType.Friendly;

        //movementType = MovementType.Chasing;
        detectionLayer = LayerMask.GetMask("Player", "Enemy");

        maxDespawnTime = 999;
    }

    void Update()
    {
        DetectCharacters();

        DetectCreatures();

        DetectHits();
    }

    private void DetectCharacters()
    {
        charDetect = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        if (charDetect.Length > 0)
        {
            bool playerFound = false;
            float distance = float.MaxValue;
            for (int i = 0; i < charDetect.Length; i++)
            {
                if (charDetect[i].gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    closestPlayer = charDetect[i].transform;
                    playerFound = true;
                }
                else if (Vector3.Distance(charDetect[i].transform.position, transform.position) < distance)
                {
                    distance = Vector3.Distance(charDetect[i].transform.position, transform.position);
                    closestEnemy = charDetect[i].transform;
                }
            }
            if (closestEnemy != null && playerFound)
            {
                Move(closestEnemy);
            }
            else if (closestPlayer != null)
            {
                Move(closestPlayer);
            }
            else
            {
                Move();
            }

        }
        else
        {
            if (closestPlayer != null)
            {
                Move(closestPlayer);
            }
            Move();
        }

    }

    private void DetectCreatures()
    {
        LayerMask creatureMask = LayerMask.GetMask("Ally");
        creatureDetect = Physics.OverlapSphere(transform.position,1, creatureMask);

        if (creatureDetect.Length>0)
        {
            if (creatureDetect.Length == 1 && creatureDetect[0].transform == transform)
            {
                return;
            }

            for (int i=0;i< creatureDetect.Length;i++)
            {
                float distance = Vector3.Distance(creatureDetect[i].transform.position, transform.position); 

                if (distance<1)
                {
                    creatureDetect[i].transform.position += (creatureDetect[i].transform.position - transform.position).normalized * 0.01f * Mathf.Clamp(1-distance,0,1); //0.0675
                }
            }
            
        }

    }

    private void DetectHits()
    {
        hits = Physics.OverlapSphere(transform.position, 1, hitLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Attacks currentAttack = hits[i].GetComponent<Attacks>();
                if (currentAttack.attackType == Attack.Targeted && currentAttack.target == transform) // && targeted
                {
                    Destroy(hits[i].gameObject);
                    TakeDamage(currentAttack.damage);
                }
                else if (currentAttack.attackType == Attack.FirstTarget)
                {
                    Destroy(hits[i].gameObject);
                    TakeDamage(currentAttack.damage);
                }
                else if (currentAttack.attackType == Attack.AreaOfEffect)
                {
                    TakeDamage(currentAttack.damage);
                }

                UIManager.singleton.UpdateAllies();
            }
        }
    }

    public void AddAlly()
    {
        if (!allyAdded)
        {
            UIManager.singleton.AddAlly(this);
            allyAdded = true;
        }
    }
}
