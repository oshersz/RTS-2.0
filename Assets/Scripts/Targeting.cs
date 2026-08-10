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

                CharacterVisual.singleton.MoveVFX(new Vector3(raycastHit.point.x, 0.25f, raycastHit.point.z), 1.5f, Color.red);

                CM.FollowTarget(targetedEnemy.transform, attackRange);

            }
            else if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, interactableLayer)) // checking if clicked on NPC
            {
                //raycastHit.transform.GetComponent<Interactable>().Interact();

                if (Vector3.Distance(transform.position,raycastHit.transform.position)<3)
                {
                    raycastHit.transform.GetComponent<Interactable>().Interact();
                }

                CM.FollowTarget(raycastHit.transform, 2, raycastHit.transform.GetComponent<Interactable>());

                //I don't remember why I did this part, maybe to show the name

                /*
                if (raycastHit.transform.TryGetComponent(out targetedEnemy))
                {
                    if (targetedEnemy != null)
                    {
                        targetedEnemy = raycastHit.transform.GetComponent<Enemy>();
                        UIManager.singleton.CurrentEnemy(targetedEnemy);
                        //CM.FollowTarget(targetedEnemy.transform, 2);
                    }
                }
                */

                if (raycastHit.transform.GetComponent<Enemy>() != null)
                {
                    UIManager.singleton.CurrentEnemy(raycastHit.transform.GetComponent<Enemy>());
                }
                
                CharacterVisual.singleton.MoveVFX(new Vector3(raycastHit.point.x, 0.25f, raycastHit.point.z), 1, Color.blue);
            }
            else
            {
                if (activeSpellIndex == -1)
                {
                    if (targetedEnemy!=null)
                    {
                        CM.FollowTarget(transform, attackRange);
                    }
                    else
                    {
                        CM.FollowTarget(null, attackRange);
                    }
                    targetedEnemy = null;
                    UIManager.singleton.CurrentEnemy(targetedEnemy);

                    
                }
            }
            if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask) && activeSpellIndex>=0)
            {
                CharacterVisual.singleton.TurnOffIndicators();

                Vector3 spellDirection = new Vector3(raycastHit.point.x - transform.position.x,0,raycastHit.point.z-transform.position.z); // making sure we don't get rotation on the X axis
                spellDirection.Normalize();

                if ((Vector3.Distance(raycastHit.point, transform.position)*2) > activeSpell.spellRadius)
                {
                    raycastHit.point = transform.position + spellDirection * (activeSpell.spellRadius / 2f);
                }

                CharacterVisual.singleton.MoveVFX(new Vector3(raycastHit.point.x, 0.25f, raycastHit.point.z), 1.5f, Color.red);

                //if spell is AOE/first hit spell do something

                CharacterStats.singleton.currentMana -= activeSpell.manaCost;

                spellsCooldown[activeSpellIndex] = Time.time + activeSpell.spellCooldown;
                UIManager.singleton.UpdateSkillCooldown(activeSpellIndex, spellsCooldown[activeSpellIndex]);
                spellDelay = Time.time + activeSpell.spellDelay;
                CharacterMovement.charState = State.AttackStun;

                if (activeSpell.spellType == Spells.Spell.FirstTarget)
                {
                    GameObject tempSkillShotSpell = Instantiate(spells[activeSpellIndex], new Vector3(transform.position.x, spells[activeSpellIndex].transform.position.y, transform.position.z),Quaternion.LookRotation(spellDirection)); //directionalTargetingIndicator.transform.rotation
                    tempSkillShotSpell.GetComponent<Spells>().damage *= (1 + CharacterStats.singleton.characterStats[(int)Stats.SpellDamage]); //consider changing
                }

                if (activeSpell.spellType == Spells.Spell.Positional)
                {
                    GameObject tempAOESpell = Instantiate(spells[activeSpellIndex], new Vector3(raycastHit.point.x, spells[activeSpellIndex].transform.position.y, raycastHit.point.z), transform.rotation); // AOE spell
                    tempAOESpell.GetComponent<Spells>().damage *= (1 + CharacterStats.singleton.characterStats[(int)Stats.SpellDamage]); //consider changing
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
        //consider changing
        if (targetedEnemy == null)
            return;

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
                CharacterVisual.singleton.TurnOffIndicators();
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

            CharacterVisual.singleton.TurnOffIndicators();
            CharacterVisual.singleton.SetIndicator(RangeIndicatorType.characterStatic, new Vector3(activeSpell.spellRadius, 1, activeSpell.spellRadius), true);

            if (activeSpell.spellType == Spells.Spell.FirstTarget)
            {
                CharacterVisual.singleton.SetIndicator(RangeIndicatorType.characterDirectional,Vector3.one,true);
            }
            else if (activeSpell.spellType == Spells.Spell.Positional)
            {
                CharacterVisual.singleton.SetIndicator(RangeIndicatorType.worldPos, Vector3.one, true);
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
