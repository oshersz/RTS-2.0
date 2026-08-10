
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats singleton { get;private set; }

    //public static const int NumberOfStats = 13;

    public int level;
    public float maxHealth;
    [HideInInspector]public float currentHealth;
    public float maxMana;
    [HideInInspector]public float currentMana;
    private float manaRegen;
    private float healthRegen;
    public float currentExp;
    [HideInInspector] public float expNeededForNextLevel;
    [HideInInspector] public float levelStartingExp;
    public float gold;
    [SerializeField] ParticleSystem levelUpVFX;

    private bool dead = false;

    private  Collider[] hits;
    private LayerMask playerHitLayer;


    private float defaultMoveSpeed;

    /** // option A
    public int statPointsToSpend;
    public int vitality;
    public int magic;
    public int dexterity;
    public int luck;
    */

    /** // option B
    [HideInInspector] public float damageGainedFromEquipment;
    [HideInInspector] public float attackSpeedGainedFromEquipment;
    [HideInInspector] public float attackRangeGainedFromEquipment;
    [HideInInspector] public float healthGainedFromEquipment;
    [HideInInspector] public float manaGainedFromEquipment;
    [HideInInspector] public float physDefGainedFromEquipment;
    [HideInInspector] public float magDefGainedFromEquipment;
    [HideInInspector] public float moveSpeedGainedFromEquipment;
    [HideInInspector] public float dodgeChanceGainedFromEquipment;
    [HideInInspector] public float goldIncreaseGainedFromEquipment;
    [HideInInspector] public float expIncreaseGainedFromEquipment;
    [HideInInspector] public float lootChanceIncreaseGainedFromEquipment;
    [HideInInspector] public float spellDamageGainedFromEquipment;
    */
    // option C
    [HideInInspector] public float[] characterStats; 


    //add health + mana sliders
    //add enemy loot and enemy give exp
    //make spells cost mana

    private void Awake()
    {
        singleton = this;
        playerHitLayer = LayerMask.GetMask("Enemy Weapon");
        level = 1;
        expNeededForNextLevel = 10;
        levelStartingExp = 0;

        characterStats = new float[13]; //13 is the number of stats
        defaultMoveSpeed = 5;
        characterStats[(int)Stats.MoveSpeed] = defaultMoveSpeed; //starting movespeed
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        manaRegen = maxMana / 25f;
        healthRegen = maxHealth / 50f;

        UIManager.singleton.UpdateCharacterStats();
    }

    void Update()
    {
        HealthManaRegen();
        if (!dead)
            DamageScan();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            GetExp(5);
        }
    }

    private void HealthManaRegen()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegen * Time.deltaTime;
            if (currentMana > maxMana)
                currentMana = maxMana;
        }

        if (currentHealth<maxHealth)
        {
            currentHealth += healthRegen * Time.deltaTime;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }

    private void DamageScan()
    {
        hits = Physics.OverlapSphere(transform.position, 1, playerHitLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Attacks currentAttack = hits[i].GetComponent<Attacks>();

                //second if might be redundant

                if (currentAttack.attackType == Attack.Targeted && currentAttack.target == transform) // && targeted
                {
                    Destroy(hits[i].gameObject);
                    //TakeDamage(currentAttack.damage);
                }
                else if (currentAttack.attackType == Attack.FirstTarget)
                {
                    Destroy(hits[i].gameObject);
                    //TakeDamage(currentAttack.damage);
                }
                else if (currentAttack.attackType == Attack.AreaOfEffect)
                {
                    //TakeDamage(currentAttack.damage);
                }
                if (Random.Range(0,100) > characterStats[(int)Stats.DodgeChance])
                {
                    //if you weren't able to dodge
                    TakeDamage(currentAttack.damage,DamageType.RangedPhysical); 
                }
            }
        }
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        if (damageType == DamageType.MeleePhysical)
        {
            currentHealth -= Mathf.Max(0, damage - characterStats[(int)Stats.PhysicalDefense]);
        }
        else if (damageType == DamageType.RangedPhysical)
        {
            //Debug.Log(characterStats[(int)Stats.PhysicalDefense]);
           // Debug.Log(Mathf.Max(0, (int)(damage - (characterStats[(int)Stats.PhysicalDefense / 3]))));
            currentHealth -= Mathf.Max(0, (int)(damage - (characterStats[(int)Stats.PhysicalDefense] / 3)));
        }
        else
        {
            currentHealth -= Mathf.Max(0, damage - characterStats[(int)Stats.MagicalDefense]);
        }
        



        if (damage / maxHealth > 0.1) //%10 of max hp
        {
            CameraShaker.singleton.ShakeCamera(Shake.Strong);
        }
        else if (damage / maxHealth > 0.05) //%5 of max hp
        {
            CameraShaker.singleton.ShakeCamera(Shake.Medium);
        }
        else 
        {
            CameraShaker.singleton.ShakeCamera(Shake.Weak);
        }




        if (currentHealth<=0)
        {
            Debug.Log("dead");
            dead = true;
            transform.Rotate(90, 0, 0);
            GetComponent<CharacterMovement>().enabled = false;
            GetComponent<Targeting>().enabled = false;
        }
    }

    public void GetExp(int exp)
    {
        currentExp += exp;
        if (currentExp > expNeededForNextLevel)
            LevelUp();
        UIManager.singleton.UpdateExpBar();
    }

    public void LevelUp()
    {
        level++;
        levelStartingExp = expNeededForNextLevel;
        expNeededForNextLevel = expNeededForNextLevel + (10 + level * level);
        levelUpVFX.Play();
        UIManager.singleton.OpenLevelUpWindow();
    }

    public void LevelUpSelect(LevelUpOptions levelUpSelection)
    {
        if (levelUpSelection == LevelUpOptions.Health)
        {
            UpgradeHealth();
        }
        else if (levelUpSelection == LevelUpOptions.Mana)
        {
            UpgradeMana();
        }
        else if (levelUpSelection == LevelUpOptions.PhysicalDefense)
        {
            characterStats[(int)Stats.PhysicalDefense] += 1;
        }
        else if (levelUpSelection == LevelUpOptions.MagicalDefense)
        {
            characterStats[(int)Stats.MagicalDefense] += 2;
        }
        else if (levelUpSelection == LevelUpOptions.MoveSpeed)
        {
            //re-calculating movespeed
            characterStats[(int)Stats.MoveSpeed] -= defaultMoveSpeed;
            defaultMoveSpeed *= 1.25f; //*1.25f
            characterStats[(int)Stats.MoveSpeed] += defaultMoveSpeed;
        }
        else if (levelUpSelection == LevelUpOptions.Dodge)
        {
            characterStats[(int)Stats.DodgeChance] += 10;
        }
        else if (levelUpSelection == LevelUpOptions.Gold)
        {
            characterStats[(int)Stats.GoldIncrease] += 25;
        }
        else if (levelUpSelection == LevelUpOptions.Exp)
        {
            characterStats[(int)Stats.ExpIncrease] += 20;
        }
        else if (levelUpSelection == LevelUpOptions.LootChanceIncrease)
        {
            characterStats[(int)Stats.LootChanceIncrease] += 10;
        }
        else if (levelUpSelection == LevelUpOptions.SpellDamage)
        {
            characterStats[(int)Stats.SpellDamage] += 15;
        }
        UIManager.singleton.UpdateCharacterStats();
        UIManager.singleton.CloseLevelUpWindow();
    }

    private void UpgradeHealth()
    {
        maxHealth *= 1.25f;
        healthRegen = maxHealth / 50f;
    }

    private void UpgradeMana()
    {
        maxMana *= 1.35f;
        manaRegen = maxMana / 25f;
    }

    public void UpdateStatsGainedFromEquipment(Equipment[] equippedItemsArray)
    {
        //making sure no previous value remained
        for (int i=0;i<characterStats.Length;i++)
        {
            characterStats[i] = 0;
        }
        for (int i=0;i<equippedItemsArray.Length;i++)
        {
            if (equippedItemsArray[i]!= null)
            {
                for (int j = 0; j<characterStats.Length;j++)
                {
                    characterStats[j] += equippedItemsArray[i].stats[j];
                }
            }
        }

        characterStats[(int)Stats.MoveSpeed] += defaultMoveSpeed; //starting movespeed
        /**
        damageGainedFromEquipment = 0;
        attackSpeedGainedFromEquipment = 0;
        attackRangeGainedFromEquipment = 0;
        healthGainedFromEquipment = 0 ;
        manaGainedFromEquipment = 0;
        physDefGainedFromEquipment = 0;
        magDefGainedFromEquipment = 0;
        moveSpeedGainedFromEquipment = 0;
        dodgeChanceGainedFromEquipment = 0;
        goldIncreaseGainedFromEquipment = 0;
        expIncreaseGainedFromEquipment = 0;
        lootChanceIncreaseGainedFromEquipment = 0;
        spellDamageGainedFromEquipment = 0;
        for (int i=0; i<equippedItemsArray.Length;i++)
        {
            if (equippedItemsArray[i]!= null)
            {
                damageGainedFromEquipment += equippedItemsArray[i].damage;
                attackSpeedGainedFromEquipment += equippedItemsArray[i].attackSpeed;
                attackRangeGainedFromEquipment += equippedItemsArray[i].attackRange;
                healthGainedFromEquipment += equippedItemsArray[i].health;
                manaGainedFromEquipment += equippedItemsArray[i].mana;
                physDefGainedFromEquipment += equippedItemsArray[i].physDef;
                magDefGainedFromEquipment += equippedItemsArray[i].magDef;
                moveSpeedGainedFromEquipment += equippedItemsArray[i].moveSpeed;
                dodgeChanceGainedFromEquipment += equippedItemsArray[i].dodgeChance;
                goldIncreaseGainedFromEquipment += equippedItemsArray[i].goldIncrease;
                expIncreaseGainedFromEquipment += equippedItemsArray[i].expIncrease;
                lootChanceIncreaseGainedFromEquipment += equippedItemsArray[i].lootChanceIncrease;
                spellDamageGainedFromEquipment += equippedItemsArray[i].spellDamage;
            }
            else
            {
                //Debug.Log("missing equipped item in index" + i);
            }

        }
        */
        UIManager.singleton.UpdateCharacterStats();
        UIManager.singleton.HideEquipmentPopup();
    }
}

public enum Stats //length 13
{
    Damage,
    AttackSpeed,
    AttackRange,
    Health,
    Mana,
    PhysicalDefense,
    MagicalDefense,
    MoveSpeed,
    DodgeChance,
    GoldIncrease,
    ExpIncrease,
    LootChanceIncrease,
    SpellDamage
}
