using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton { get; private set; }
    [Header("Enemy Selection")]
    [SerializeField] GameObject enemyUI;
    [SerializeField] TextMeshProUGUI enemyName;
    [SerializeField] Slider enemyHP;
    private Creature currentEnemy;
    [SerializeField] GameObject targetingArrow;
    [SerializeField] ParticleSystem selectVFX;

    [Header("Character Stats")]
    [SerializeField] Slider characterHp;
    [SerializeField] Slider characterMana;
    [SerializeField] Slider characterExp;
    public TextMeshProUGUI characterGold;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject levelUpWindow;
    [SerializeField] GameObject characterWindow;
    [SerializeField] Image[] characterEquipment;
    [SerializeField] TextMeshProUGUI[] characterStats;
    [SerializeField] GameObject equipmentPopup;
    [SerializeField] TextMeshProUGUI[] equipmentPopupStats;

    [Header("Skills")]
    [SerializeField] Slider QSkill;
    private float QSkillStarterCooldown;
    private float QSkillNextCooldown;
    [SerializeField] Slider WSkill;
    private float WSkillStarterCooldown;
    private float WSkillNextCooldown;

    [Header("Allies")]
    [SerializeField] GameObject allyUI;
    [SerializeField] GameObject allyWindow;
    //List<Ally> allies = new List<Ally>();
    //List<GameObject> alliesUI = new List<GameObject>();
    //private int allyCount;
    List<AllyUI> allies = new List<AllyUI>();

    public static float framer;

    public struct AllyUI
    {
        public Ally ally;
        public GameObject UI;
        public AllyUI(Ally ally, GameObject UI)
        {
            this.ally = ally;
            this.UI = UI;
        }
    }




    private void Awake()
    {
        singleton = this;
    }
    void Update()
    {
        characterHp.value = Mathf.InverseLerp(0, CharacterStats.singleton.maxHealth, CharacterStats.singleton.currentHealth);
        characterMana.value = Mathf.InverseLerp(0, CharacterStats.singleton.maxMana, CharacterStats.singleton.currentMana);


        if (currentEnemy != null)
        {
            enemyHP.value = Mathf.InverseLerp(0, currentEnemy.maxHp, currentEnemy.currentHp);
            targetingArrow.transform.position = currentEnemy.transform.position;
        }
        else
            targetingArrow.SetActive(false);

        if (Time.time < QSkillNextCooldown)
        {
            QSkill.value = Mathf.InverseLerp(QSkillStarterCooldown, QSkillNextCooldown, Time.time);
        }
        if (Time.time < WSkillNextCooldown)
        {
            WSkill.value = Mathf.InverseLerp(WSkillStarterCooldown, WSkillNextCooldown, Time.time);
        }

        OpenCharacterWindow();
        OpenInventory();


        if (framer++%60 == 0)
        {
            UpdateAllies();
        }
        
    }

    public void CurrentEnemy(Creature targetedEnemy)
    {
        if (targetedEnemy!= null)
        {
            enemyUI.SetActive(true);
            targetingArrow.SetActive(true);
            if (currentEnemy!= targetedEnemy)
                selectVFX.Play();
            currentEnemy = targetedEnemy;
            enemyName.text = currentEnemy.creatureName;
            enemyHP.value = Mathf.InverseLerp(0, currentEnemy.maxHp, currentEnemy.currentHp);
        }
        else
        {
            enemyUI.SetActive(false);
            targetingArrow.SetActive(false);

        }
    }

    public void AddAlly(Ally ally)
    {
        if (!allyWindow.activeSelf)
        {
            allyWindow.SetActive(true);
        }
        GameObject newAllyUI = Instantiate(allyUI, allyWindow.transform);
        allies.Add(new AllyUI(ally,newAllyUI));
        
        ally.creatureName = "NPC Ally " + allies.Count;

        //newAllyUI.name = "NPC Ally " + allies.Count;
        newAllyUI.GetComponent<TextMeshProUGUI>().text = "NPC Ally "+ allies.Count;
        newAllyUI.GetComponent<Slider>().value = Mathf.InverseLerp(0, ally.maxHp, ally.currentHp);
    }

    public void UpdateAllies()
    {
        for (int i = allies.Count - 1; i>=0;i--) //allies.Count -1 //allyWindow.transform.childCount -1
        {
            if (allies[i].ally!=null || allies[i].UI!=null)
            {
                if (allies[i].ally == null)
                {
                    Destroy(allies[i].UI);
                    allies.RemoveAt(i);
                }
                else
                {
                    allies[i].UI.GetComponent<Slider>().value = Mathf.InverseLerp(0, allies[i].ally.maxHp, allies[i].ally.currentHp);
                }
            }


        }
        if (allyWindow.transform.childCount == 0)
        {
            allyWindow.SetActive(false);
        }
    }

    public void TeleportAllies(Vector3 teleportPosition)
    {
        for (int i = allies.Count - 1; i >= 0; i--) //allies.Count -1 //allyWindow.transform.childCount -1
        {
            allies[i].ally.transform.position = teleportPosition;
        }
    }

    public void UpdateSkillCooldown(int skillIndex, float skillCurrentCooldown, float skillMaxCooldown)
    {
        if (skillIndex == 0)
        {
            QSkill.value = Mathf.InverseLerp(0, skillCurrentCooldown, skillMaxCooldown);
        }
        else if (skillIndex == 1)
        {
            WSkill.value = Mathf.InverseLerp(0, skillCurrentCooldown, skillMaxCooldown);
        }
    }

    public void UpdateSkillCooldown(int skillIndex, float skillNextCooldown)
    {
        if (skillIndex == 0)
        {
            QSkillStarterCooldown = Time.time;
            QSkillNextCooldown = skillNextCooldown;
        }
        else if (skillIndex == 1)
        {
            WSkillStarterCooldown = Time.time;
            WSkillNextCooldown = skillNextCooldown;
        }
    }

    public void UpdateExpBar()
    {
        characterExp.value = Mathf.InverseLerp(CharacterStats.singleton.levelStartingExp, CharacterStats.singleton.expNeededForNextLevel, CharacterStats.singleton.currentExp);
    }

    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryWindow.gameObject.activeSelf)
            {
                inventoryWindow.gameObject.SetActive(false);
                HideEquipmentPopup();
                UpdateCharacterStats();
            }
            else
                inventoryWindow.gameObject.SetActive(true);
        }
    }

    private void OpenCharacterWindow()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (characterWindow.activeSelf)
                characterWindow.SetActive(false);
            else
                characterWindow.SetActive(true);
        }
    }

    public void OpenLevelUpWindow()
    {
        levelUpWindow.SetActive(true);
    }

    public void CloseLevelUpWindow()
    {
        levelUpWindow.SetActive(false);
    }

    public void EquipItem(Sprite equipmentSprite,int index)
    {
        characterEquipment[index].sprite = equipmentSprite;

        if (index == (int)EquipmentType.Boots) //double  sprite for boots
                characterEquipment[7].sprite = equipmentSprite;
        if (index == (int)EquipmentType.Ring1) //double  sprite for earrings
            characterEquipment[8].sprite = equipmentSprite;


    }

    public void UpdateCharacterStats()
    {
        for (int i=0;i<characterStats.Length;i++)
        {
            characterStats[i].text = Enum.GetName(typeof(Stats), i) +":  " + CharacterStats.singleton.characterStats[i]; 
        }

        //needs refactoring
        /**
        characterStats[0].text = "Weapon Damage: " + CharacterStats.singleton.damageGainedFromEquipment;
        characterStats[1].text = "Attack Speed: " + CharacterStats.singleton.attackSpeedGainedFromEquipment;
        characterStats[2].text = "Attack Range: " + CharacterStats.singleton.attackRangeGainedFromEquipment;
        characterStats[3].text = "Health: " + (CharacterStats.singleton.maxHealth) + " + " + CharacterStats.singleton.healthGainedFromEquipment;
        characterStats[4].text = "Mana: " + (CharacterStats.singleton.maxMana) + " + " + CharacterStats.singleton.manaGainedFromEquipment;
        characterStats[5].text = "Physical Defense: " + CharacterStats.singleton.physDefGainedFromEquipment;
        characterStats[6].text = "Magical Defense: " + CharacterStats.singleton.magDefGainedFromEquipment;
        characterStats[7].text = "Move Speed: 5 + " + CharacterStats.singleton.moveSpeedGainedFromEquipment; //needs to get the starting movespeed value
        characterStats[8].text = "Dodge Chance: " + CharacterStats.singleton.dodgeChanceGainedFromEquipment;
        characterStats[9].text = "Gold Increase %: " + CharacterStats.singleton.goldIncreaseGainedFromEquipment;
        characterStats[10].text = "Exp Increase %: " + CharacterStats.singleton.expIncreaseGainedFromEquipment;
        characterStats[11].text = "Rare Loot Chance %: " + CharacterStats.singleton.lootChanceIncreaseGainedFromEquipment;
        characterStats[12].text = "Spell Damage: " + CharacterStats.singleton.spellDamageGainedFromEquipment;
        */

    }

    public void CompareStats(float[] comparisonArray)
    {
        //showing the results as saved in the array
        for (int i = 0; i < characterStats.Length; i++)
        {
            if (comparisonArray[i] > 0)
            {
                characterStats[i].text = Enum.GetName(typeof(Stats), i) + ":  " + CharacterStats.singleton.characterStats[i] + " + " + comparisonArray[i];
            }
            else if (comparisonArray[i] < 0)
            {
                characterStats[i].text = Enum.GetName(typeof(Stats), i) + ":  " + CharacterStats.singleton.characterStats[i] + " - " + (-1 *comparisonArray[i]); //putting the '-' where I want it
            }
            else
            {
                characterStats[i].text = Enum.GetName(typeof(Stats), i) + ":  " + CharacterStats.singleton.characterStats[i];
            }
        }
    }

    public void CompareStats(string[] comparisonArray)
    {
        //for some reason it didn't work so trying the other way
        for (int i = 0; i < characterStats.Length; i++)
        {
            characterStats[i].text = Enum.GetName(typeof(Stats), i) + ":  " + CharacterStats.singleton.characterStats[i] + " " + comparisonArray[i].ToString();
        }
    }

    public void ShowEquipmentPopup(Equipment equipmentToShow,Vector3 windowPosition)
    {
        equipmentPopup.transform.position = windowPosition;
        equipmentPopup.SetActive(true);
        for (int i = 0; i<13;i++)
        {
            if (equipmentToShow.stats[i]== 0)
            {
                equipmentPopupStats[i].gameObject.SetActive(false);
            }
            else
            {
                equipmentPopupStats[i].gameObject.SetActive(true);
                equipmentPopupStats[i].text = Enum.GetName(typeof(Stats), i) + " " + equipmentToShow.stats[i].ToString();
            }
        }
        //reserved for the equipment name
        equipmentPopupStats[13].text = equipmentToShow.itemName;
        //changing the name color based on the rarity of the item - common = grey, rare = blue, scarce = purple, nonexistent = orange
        // the colors I wanted doesn't exist in the preset colors
        if (equipmentToShow.rarity == LootDrop.Common)
        {
            equipmentPopupStats[13].color = Color.grey;
        }
        else if (equipmentToShow.rarity == LootDrop.Rare)
        {
            equipmentPopupStats[13].color = Color.green;
        }
        else if (equipmentToShow.rarity == LootDrop.Scarce)
        {
            equipmentPopupStats[13].color = Color.cyan;
        }
        else if (equipmentToShow.rarity == LootDrop.Nonexistent)
        {
            equipmentPopupStats[13].color = Color.red;
        }
    }

    public void HideEquipmentPopup()
    {
        equipmentPopup.SetActive(false);
    }
}
