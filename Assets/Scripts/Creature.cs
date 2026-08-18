using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    [Header("Enemy Behavior")]

    public MovementType movementType;
    public BehaviorType behaviorType;
    public DetectionType detectionType;
    private DetectionType startingDetectionType;
    public LootDrop lootDrop;
    public LayerMask detectionLayer;
    public LayerMask hitLayer;
    //implement creature attacking
    [Header("Enemy Stats")]

    public Equipment creatureWeapon;
    public string enemyName;
    public float maxHp;
    private float idleHealingTimer;
    [HideInInspector]public float currentHp;
    public float moveSpeed;
    public float detectionRadius;
    public float maxDespawnTime;
    [HideInInspector]public float currentDespawnTime;
    private float attackCD;
    private float attackDamage;

    private Transform lastTarget;

    [HideInInspector]public Vector3 startingPosition;
    private bool atStartingPosition = true;

    [Header("Enemy Visual")]
    private Animator anim;

    private void Awake()
    {
        currentHp = maxHp;
        currentDespawnTime = maxDespawnTime;
        startingDetectionType = detectionType;
        anim = GetComponent<Animator>();
        if (creatureWeapon!=null)
        {
            attackCD = Time.time + (1f / creatureWeapon.stats[(int)Stats.AttackSpeed]);
            attackDamage = creatureWeapon.stats[(int)Stats.Damage];
        }
    }

    public enum MovementType
    {
        Idle,
        Chasing,
        Running,
        LinearPatroling,
        CircularPatroling
    }
    
    public enum BehaviorType
    {
        Agressive,
        Neutral,
        Friendly,
        FrightenedOnAttack,
        FrightenedAlways
    }

    public enum DetectionType
    {
        OnRadius,
        OnLineOfSight
    }

    public virtual void Move()
    {
        RegenHp(); //enemies regenerate hp while not aggroed
        if (movementType == MovementType.Idle)
        {
            //do nothing
        }
        else if (movementType == MovementType.Chasing)
        {
            if (!atStartingPosition)
                ReturnToStartingPosition();
        }
        else if (movementType == MovementType.Running)
        {
            if (!atStartingPosition)
                ReturnToStartingPosition();
        }
        else if (movementType == MovementType.LinearPatroling)
        {
            if (!atStartingPosition)
                ReturnToStartingPosition();
            else
                LinearPatrol();
        }
        else if (movementType == MovementType.CircularPatroling)
        {
            if (!atStartingPosition)
                ReturnToStartingPosition();
            else
                CircularPatrol();
        }
    } //movement if the player is NOT in detection radius

    public virtual void Move(Transform target)
    {
        currentDespawnTime = maxDespawnTime; //enemy can't despawn while interacting with player
        if (movementType == MovementType.Idle)
        {
            transform.LookAt(target);
            //do nothing
        }
        else if (movementType == MovementType.Chasing)
        {
            Chase(target);
        }
        else if (movementType == MovementType.Running)
        {
            Run(target);
        }
        else if (movementType == MovementType.LinearPatroling)
        {
            LinearPatrol(target);
        }
        else if (movementType == MovementType.CircularPatroling)
        {
            CircularPatrol(target);
        }
    } //movement if the player IS in player radius

    public virtual void ReturnToStartingPosition()
    {
        if (Vector3.Distance(startingPosition, transform.position) > 2)
        {
            Vector3 moveDirection = startingPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            transform.Translate(Vector3.forward * moveSpeed * 0.5f * Time.deltaTime);
        }
        else
        {
            atStartingPosition = true;
            if (detectionType != startingDetectionType) // enemy chasing on radius is reset when he returns to his starting location
                detectionType = startingDetectionType;
        }
    }

    public virtual void Chase(Transform target)
    {
        if (detectionType == DetectionType.OnLineOfSight && !Physics.Raycast(transform.position, transform.forward, detectionRadius, detectionLayer))
        {
            return;
        }

        if (Vector3.Distance(target.position,transform.position)<detectionRadius && Vector3.Distance(target.position, transform.position) > 1.5f)
        {
            idleHealingTimer = 0; //passive healing timer is reset
            target.position = new Vector3(target.position.x, transform.position.y, target.position.z); //preventing accidental Y movement
            transform.LookAt(target);
            //if hostile & possible to attack
            if ((behaviorType == BehaviorType.Agressive || behaviorType == BehaviorType.Friendly) && creatureWeapon!= null && creatureWeapon.stats[(int)Stats.AttackRange] >= Vector3.Distance(target.position, transform.position)) //if in range to attack
            {
                if (behaviorType == BehaviorType.Friendly && target.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return;
                }

                if (Time.time>attackCD)
                {
                    attackCD = Time.time + (1f / creatureWeapon.stats[(int)Stats.AttackSpeed]);
                    if (creatureWeapon.weaponType == WeaponType.Bow)
                    {
                        /* transferred to animation event
                        GameObject tempProjectile = Instantiate(creatureWeapon.projectilePrefab, transform.position, transform.rotation, null);
                        tempProjectile.GetComponent<Attacks>().target = target.transform;
                        tempProjectile.GetComponent<Attacks>().damage = attackDamage;
                        */
                        lastTarget = target;
                        anim.SetTrigger("BowAttack");
                    }
                    else if (creatureWeapon.weaponType == WeaponType.Sword)
                    {
                        // trying to do this with animation events to coordinate with the attack timing
                        //CharacterStats.singleton.currentHealth -= attackDamage;
                        //CharacterStats.singleton.TakeDamage(attackDamage);
                        anim.SetTrigger("MeleeAttack");
                        lastTarget = target;
                        //plan b
                        /**
                        if (target.TryGetComponent<CharacterStats>(out CharacterStats heroStats))
                        {
                            heroStats.currentHealth -= attackDamage;
                        }
                        //target.GetComponent<CharacterStats>().TakeDamage();
                        //targetedEnemy.TakeDamage(attackDamage);
                        */
                    }
                }
            }
            else
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                atStartingPosition = false;
            }
        }
    }
    public virtual void Run(Transform target)
    {
        if (Vector3.Distance(target.position, transform.position) < detectionRadius)
        {
            idleHealingTimer = 0; //passive healing timer is reset
            target.position = new Vector3(target.position.x, transform.position.y, target.position.z); //preventing accidental Y movement
            transform.LookAt(target);
            transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+180, transform.rotation.eulerAngles.z);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            atStartingPosition = false;
        }
    }

    private bool patrolDir;

    public virtual void LinearPatrol()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if (Mathf.Sin((Time.time/Mathf.PI)*3.5f)>0 && !patrolDir)
        {
            transform.forward = -transform.forward;
            patrolDir = !patrolDir;
        }
        else if (Mathf.Sin((Time.time / Mathf.PI) * 3.5f) <= 0 && patrolDir)
        {
            transform.forward = -transform.forward;
            patrolDir = !patrolDir;
        }
    }

    public virtual void LinearPatrol(Transform target)
    {
        bool canSeeTarget = true;
        if (detectionType == DetectionType.OnLineOfSight && !Physics.Raycast(transform.position, transform.forward, detectionRadius, detectionLayer))
        {
            canSeeTarget = false;
        }
        if (Vector3.Distance(target.position, transform.position) < detectionRadius && canSeeTarget)
        {
            Chase(target);
        }
        else
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            if ((int)Time.time % 5 == 0)
            {
                transform.forward = -transform.forward;
            }
        }
    }

    public virtual void CircularPatrol()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        transform.Rotate(0, 45 * Time.deltaTime, 0);
    }

    public virtual void CircularPatrol(Transform target)
    {
        bool canSeeTarget = true;
        if (detectionType == DetectionType.OnLineOfSight && !Physics.Raycast(transform.position, transform.forward, detectionRadius, detectionLayer))
        {
            canSeeTarget = false;
        }
        if (Vector3.Distance(target.position, transform.position) < detectionRadius && canSeeTarget )
        {
            Chase(target);
        }
        else
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            transform.Rotate(0, 45 * Time.deltaTime, 0);
        }
    }

    public virtual void TakeDamage()
    {
        currentHp--;
        if (currentHp <= 0)
        {
            LootManager.singleton.DropLoot(lootDrop,transform.position);
            CharacterStats.singleton.GetExp(ExpCalc(lootDrop));
            Destroy(gameObject);
        }
    }

    /*
    public virtual void DropLoot()
    {
        if (lootDrop == LootDrop.Common)
        {
            //LootManager.DropLoot(lootDrop)
        }
        else if (lootDrop == LootDrop.Rare)
        {
            //
        }
        else if (lootDrop == LootDrop.Scarce)
        {
            //
        }
        else if (lootDrop == LootDrop.Nonexistent)
        {
            //
        }
    }
    */
    public virtual void TakeDamage(float damageAmount)
    {
        currentHp-= damageAmount;
        if (currentHp <= 0)
        {
            LootManager.singleton.DropLoot(lootDrop, transform.position);
            CharacterStats.singleton.GetExp(ExpCalc(lootDrop));
            UIManager.singleton.CurrentEnemy(null);
            CharacterVisual.singleton.React(Reactions.Vicious);
            Destroy(gameObject);
        }
        else
        {
            if (behaviorType != BehaviorType.Friendly)
            {
                UIManager.singleton.CurrentEnemy(this);
            }
        }
        ChangeBehavior();
    }

    public virtual void ChangeBehavior()
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

        if (behaviorType == BehaviorType.Friendly)
        {
            movementType = MovementType.Chasing;

            //detectionLayer = LayerMask.GetMask("Player", "Enemy");
            gameObject.layer = LayerMask.NameToLayer("Ally");
        }
    }

    public virtual void RegenHp()
    {
        idleHealingTimer += Time.deltaTime;

        if (currentHp < maxHp && idleHealingTimer>3)
            currentHp += maxHp / 10f * Time.deltaTime;
    }

    public void DealDamage() //for animation event
    {
        //rethink this segment
        if (lastTarget!=null)
        {
            if (lastTarget.GetComponent<Enemy>() != null)
            {
                lastTarget.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
            else if (lastTarget.GetComponent<Ally>() != null)
            {
                lastTarget.GetComponent<Ally>().TakeDamage(attackDamage);
                UIManager.singleton.UpdateAllies();
            }
            else if (Random.Range(0, 100) > CharacterStats.singleton.characterStats[(int)Stats.DodgeChance])
            {
                //if you weren't able to dodge
                CharacterStats.singleton.TakeDamage(attackDamage, DamageType.MeleePhysical);
                CharacterVisual.singleton.React(Reactions.Damage);
            }
        }


    }

    public void ShootArrow() //for animation event
    {
        if (lastTarget!=null)
        {
            GameObject tempProjectile = Instantiate(creatureWeapon.projectilePrefab, transform.position, transform.rotation, null);
            tempProjectile.GetComponent<Attacks>().target = lastTarget.transform;
            tempProjectile.GetComponent<Attacks>().damage = attackDamage;
        }

    }

    public int ExpCalc(LootDrop enemyRarity)
    {
        int expValue = 0;
        if (enemyRarity == LootDrop.Common)
            expValue = CharacterStats.singleton.level * (int)(1+(CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease]/100));
        else if (enemyRarity == LootDrop.Rare)
            expValue = (int)(CharacterStats.singleton.level * 3 * (1+(CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease] / 100)) *  Mathf.Pow(1.04f, CharacterStats.singleton.level));
        else if (enemyRarity == LootDrop.Scarce)
            expValue = (int)(CharacterStats.singleton.level * 7 * (1+(CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease] / 100)) * Mathf.Pow(1.06f, CharacterStats.singleton.level));
        else if (enemyRarity == LootDrop.Nonexistent)
            expValue = (int)(CharacterStats.singleton.level * 20 * (1+(CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease] / 100)) * Mathf.Pow(1.1f, CharacterStats.singleton.level));
        else
            Debug.Log(transform.name + " error with enemy drop type, please fix");

        return expValue;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * detectionRadius));
    }
}
