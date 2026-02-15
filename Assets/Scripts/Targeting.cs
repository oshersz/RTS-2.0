using System;
using UnityEngine;
using TMPro;

public class Targeting : MonoBehaviour
{
    //[SerializeField] UIManager canvasUI;
    private LayerMask floorMask;
    private LayerMask enemyLayer;
    private LayerMask interactableLayer;
    private Ray raycastFromMouse;
    private RaycastHit raycastHit;

    private CharacterMovement CM;

    private Creature targetedEnemy;
    //private Creature lastTargetedEnemy;

    [Header("Weapon Related")]

    [SerializeField] Equipment equippedWeapon;

    [SerializeField] Equipment[] weaponList;
    [NamedArray(new string[] { "Sword", "Bow", "Staff"})]
    [SerializeField] GameObject[] equippedWeaponsGFX;

    private Animator charAnim;

    [Header("Visual Effects")]

    [SerializeField] ParticleSystem moveEffect;
    [SerializeField] GameObject directionalTargetingIndicator;
    [SerializeField] GameObject aerialTargetingIndicator;
    [SerializeField] GameObject radiusTargetingIndicator;
    [SerializeField] GameObject[] spells;
    private float[] spellsCooldown;
    private Spells activeSpell;

    private int activeSpellIndex = -1;

    private WeaponRangeType characterWeaponRange;
    private WeaponType characterWeaponType;

    private float damage;
    private float attackSpeed;
    private float attackRange;
    private float attackCD;
    private float attackDelay = float.MaxValue;
    private float spellDelay = 0;
    private GameObject projectilePrefab;
    

    void Start()
    {
        CM = GetComponent<CharacterMovement>();
        enemyLayer = 512; //512 is the 10th layer
        floorMask = 256;
        //interactableLayer = LayerMask.NameToLayer("Interactable");
        interactableLayer = LayerMask.GetMask("Interactable");
        charAnim = GetComponent<Animator>();

        SaveWeaponStats();

        spellsCooldown = new float[4];

        //equip starting weapon
        Inventory.singleton.EquipEquipmentItem(equippedWeapon);

    }

    void Update()
    {
        Target();

        Attack();

        SwitchWeapon();

        CastSpell();

    }


    private void Target()
    {
        if (Input.GetMouseButtonDown(0))
        {
            raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //because it's a raycast, it hits invisble colliders
            //if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, enemyLayer)) //checking if clicked on enemy
            if (Physics.BoxCast(raycastFromMouse.origin, Vector3.one*0.5f, raycastFromMouse.direction, out raycastHit,Quaternion.identity, 500, enemyLayer)) //easier clicking on moving targets
            {
                targetedEnemy = raycastHit.transform.GetComponent<Enemy>();
                UIManager.singleton.CurrentEnemy(targetedEnemy);

                //lastTargetedEnemy = targetedEnemy;

                ParticleSystem moveEffectTemp = Instantiate(moveEffect, new Vector3(raycastHit.point.x, moveEffect.transform.position.y, raycastHit.point.z), moveEffect.transform.rotation);
                moveEffectTemp.transform.localScale *= 1.5f;
                //moveEffectTemp.startColor = Color.red;
                ParticleSystem.MainModule moveEffectProperties =  moveEffectTemp.main;
                moveEffectProperties.startColor = Color.red;

                if (characterWeaponRange == WeaponRangeType.Ranged)
                {
                    CM.FollowTarget(targetedEnemy.transform, attackRange);
                    //EquippedWeapon.target = targetedEnemy.transform;
                }
                else if (characterWeaponRange == WeaponRangeType.Melee)
                {
                    CM.FollowTarget(targetedEnemy.transform, attackRange);
                    //EquippedWeapon.target = targetedEnemy.transform;
                }
                //if spell is targeted spell do something
            }
            else if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, interactableLayer)) // checking if clicked on NPC
            {
                raycastHit.transform.GetComponent<Interactable>().Interact();

                targetedEnemy = raycastHit.transform.GetComponent<Enemy>();
                UIManager.singleton.CurrentEnemy(targetedEnemy);

                ParticleSystem moveEffectTemp = Instantiate(moveEffect, new Vector3(raycastHit.point.x, moveEffect.transform.position.y, raycastHit.point.z), moveEffect.transform.rotation);
                ParticleSystem.MainModule moveEffectProperties = moveEffectTemp.main;
                moveEffectProperties.startColor = Color.blue;
            }
            else
            {
                if (activeSpellIndex == -1)
                {
                    targetedEnemy = null;
                    UIManager.singleton.CurrentEnemy(targetedEnemy);

                    if (characterWeaponRange == WeaponRangeType.Ranged)
                    {
                        CM.FollowTarget(null, attackRange);
                        //EquippedWeapon.target = null;
                    }
                    else if (characterWeaponRange == WeaponRangeType.Melee)
                    {
                        CM.FollowTarget(null, attackRange);
                        //EquippedWeapon.target = null;
                    }
                }
            }
            if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask) && activeSpellIndex>=0)
            {
                directionalTargetingIndicator.SetActive(false);
                aerialTargetingIndicator.SetActive(false);
                radiusTargetingIndicator.SetActive(false);

                //Debug.Log(Vector3.Distance(raycastHit.point, transform.position));

                if ((Vector3.Distance(raycastHit.point, transform.position)*2) > activeSpell.spellRadius)
                {
                    //return;
                    // think about how to implement that it cast the spell at max range

                    Vector3 spellDirection = raycastHit.point - transform.position;
                    spellDirection.Normalize();
                    raycastHit.point = transform.position + spellDirection * (activeSpell.spellRadius / 2f);
                }

                ParticleSystem moveEffectTemp = Instantiate(moveEffect, new Vector3(raycastHit.point.x, moveEffect.transform.position.y, raycastHit.point.z), moveEffect.transform.rotation);
                moveEffectTemp.transform.localScale *= 1.5f;
                //moveEffectTemp.startColor = Color.red;
                ParticleSystem.MainModule moveEffectProperties = moveEffectTemp.main;
                moveEffectProperties.startColor = Color.red;

                //if spell is AOE/first hit spell do something

                CharacterStats.singleton.currentMana -= activeSpell.manaCost;

                spellsCooldown[activeSpellIndex] = Time.time + activeSpell.spellCooldown;
                UIManager.singleton.UpdateSkillCooldown(activeSpellIndex, spellsCooldown[activeSpellIndex]);
                spellDelay = Time.time + activeSpell.spellDelay;
                CharacterMovement.charState = State.AttackStun;

                if (activeSpell.spellType == Spells.Spell.FirstTarget)
                {
                    GameObject tempSkillShotSpell = Instantiate(spells[activeSpellIndex], new Vector3(transform.position.x, spells[activeSpellIndex].transform.position.y, transform.position.z), directionalTargetingIndicator.transform.rotation);
                }

                if (activeSpell.spellType == Spells.Spell.Positional)
                {
                    GameObject tempAOESpell = Instantiate(spells[activeSpellIndex], new Vector3(raycastHit.point.x, spells[activeSpellIndex].transform.position.y, raycastHit.point.z), transform.rotation); // AOE spell
                }

                if (activeSpell.spellType == Spells.Spell.SelfTarget) //&& CharacterMovement.charState != State.AttackStun
                {
                    
                }

                activeSpellIndex = -1;
            }
        }

        if (targetedEnemy == null)
        {
            //UIManager.singleton.CurrentEnemy(targetedEnemy);
            //EquippedWeapon.target = null;
        }
    }

    private void Attack()
    {
        if (CharacterMovement.charState == State.Attacking && Time.time > attackCD) //starting attack sequence
        {
            attackDelay = Time.time + ((1f / attackSpeed) / 10f);
            attackCD = Time.time + (1f / attackSpeed);
            CharacterMovement.charState = State.AttackStun;
            //play attack animation

            charAnim.SetFloat("SwordSpeed", attackSpeed * 3f);
            charAnim.SetFloat("BowSpeed", attackSpeed * 3f);

            if (characterWeaponType == WeaponType.Sword)
                charAnim.SetTrigger("SwordAttack");
            else if (characterWeaponType == WeaponType.Bow)
                charAnim.SetTrigger("BowAttack");
        }

        if (Time.time > attackDelay) //the attack itself
        {
            if (characterWeaponType == WeaponType.Bow)
            {
                GameObject tempProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation, null);
                tempProjectile.GetComponent<Attacks>().target = targetedEnemy.transform;
                tempProjectile.GetComponent<Attacks>().damage = damage;
            }
            else if (characterWeaponType == WeaponType.Sword)
            {
                targetedEnemy.TakeDamage(damage);
            }
            attackDelay = float.MaxValue;

            CharacterMovement.charState = State.Attacking;

        }
    }

    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equippedWeaponsGFX[(int)characterWeaponType].SetActive(false);
            equippedWeapon = weaponList[0]; //0 is reserved for swords

            SaveWeaponStats();

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equippedWeaponsGFX[(int)characterWeaponType].SetActive(false);
            equippedWeapon = weaponList[1]; //1 is reserved for bows

            SaveWeaponStats();

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //EquippedWeapon = weaponList[2];

            /*
            characterWeaponRange = EquippedWeapon.weaponRangeType;
            characterWeaponType = EquippedWeapon.weaponType;
            attackRange = EquippedWeapon.attackRange;
            attackSpeed = EquippedWeapon.attackSpeed;
            projectilePrefab = EquippedWeapon.projectilePrefab;
            damage = EquippedWeapon.damage;
            equippedWeaponsGFX[(int)characterWeaponType].SetActive(true);
            */
        }
    }

    private void SaveWeaponStats()
    {
        characterWeaponRange = equippedWeapon.weaponRangeType;
        characterWeaponType = equippedWeapon.weaponType;
        attackRange = equippedWeapon.stats[(int)Stats.AttackRange];
        attackSpeed = equippedWeapon.stats[(int)Stats.AttackSpeed];
        projectilePrefab = equippedWeapon.projectilePrefab;
        damage = equippedWeapon.stats[(int)Stats.Damage];
        equippedWeaponsGFX[(int)characterWeaponType].SetActive(true);
    }

    public void EquipNewWeapon(Equipment weaponToEquip)
    {
        equippedWeaponsGFX[(int)characterWeaponType].SetActive(false);
        if (weaponToEquip.weaponType == WeaponType.Sword)
        {
            weaponList[0] = weaponToEquip;
            equippedWeapon = weaponList[0];
        }
        else if (weaponToEquip.weaponType == WeaponType.Bow)
        {
            weaponList[1] = weaponToEquip;
            equippedWeapon = weaponList[1];
        }
        else if (weaponToEquip.weaponType == WeaponType.Staff)
        {
            weaponList[2] = weaponToEquip;
            equippedWeapon = weaponList[2];
        }
        else
        {
            Debug.LogError("Check the weapon list or the EquipNewWeapon function");
        }

        SaveWeaponStats();

    }

    private void CastSpell()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.R))
        {
            if (activeSpellIndex>=0)
            {
                activeSpellIndex = -1;
                directionalTargetingIndicator.SetActive(false);
                aerialTargetingIndicator.SetActive(false);
                radiusTargetingIndicator.SetActive(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Time.time < spellsCooldown[0] || spells[0].GetComponent<Spells>().manaCost>CharacterStats.singleton.currentMana)
                    return;
                activeSpellIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.W) )
            {
                if (Time.time < spellsCooldown[1] || spells[1].GetComponent<Spells>().manaCost > CharacterStats.singleton.currentMana)
                    return;
                activeSpellIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.E) )
            {
                if (Time.time < spellsCooldown[2] || spells[2].GetComponent<Spells>().manaCost > CharacterStats.singleton.currentMana)
                    return;
                activeSpellIndex = 2;
            }
            else if (Input.GetKeyDown(KeyCode.R) )
            {
                if (Time.time < spellsCooldown[3] || spells[3].GetComponent<Spells>().manaCost > CharacterStats.singleton.currentMana)
                    return;
                activeSpellIndex = 3;
            }

            activeSpell = spells[activeSpellIndex].GetComponent<Spells>();

            directionalTargetingIndicator.SetActive(false);
            aerialTargetingIndicator.SetActive(false);

            radiusTargetingIndicator.SetActive(true);
            radiusTargetingIndicator.transform.localScale = new Vector3(activeSpell.spellRadius,1, activeSpell.spellRadius);


            if (activeSpell.spellType == Spells.Spell.FirstTarget)
            {
                directionalTargetingIndicator.SetActive(true);
            }
            else if (activeSpell.spellType == Spells.Spell.Positional)
            {
                aerialTargetingIndicator.SetActive(true);
            }

        }

        /*

        if (eSpellOn && Time.time< (spellDelay - 0.375f))//change later
        {
            transform.localScale /= (1 + Time.deltaTime *7.5f);
        }
        else if (Time.time < spellDelay)//eSpellOn &&  && CharacterMovement.charState == State.AttackStun change later
        {
            transform.localScale *= (1 + Time.deltaTime *7.5f);
            if (transform.localScale.magnitude > Vector3.one.magnitude)
                transform.localScale = Vector3.one;

            if (eSpellOn)
            {
                transform.position = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z); //horrible code
                eSpellOn = false;
            }
            //spellDelay = float.MinValue;
            
        }
        */

        if (Time.time > spellDelay)
        {
            CharacterMovement.charState = State.Idle;

            transform.localScale = Vector3.one;
            //eSpellOn = false;
        }
    }

}


public enum WeaponRangeType
{
    Melee,
    Ranged
}

public enum WeaponType
{
    Sword,
    Bow,
    Staff
}
