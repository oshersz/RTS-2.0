using UnityEngine;
using TMPro;

public class LevelUpSelection : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeDescription;
    public enum LevelUpOptions
    {
        Health,
        Mana,
        MoveSpeed,
        Gold
    }
    public LevelUpOptions levelUpOptions;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable() //consider transferring the upgrades themselves to here
    {
        if (levelUpOptions == LevelUpOptions.Health)
        {
            upgradeDescription.text = "Upgrades Max Health by 25%, from " + CharacterStats.singleton.maxHealth + " to " + (CharacterStats.singleton.maxHealth * 1.25f);
        }
        else if (levelUpOptions == LevelUpOptions.Mana)
        {
            upgradeDescription.text = "Upgrades Max Mana by 35%, from " + CharacterStats.singleton.maxMana + " to " + (CharacterStats.singleton.maxMana * 1.35f);
        }
        else if (levelUpOptions == LevelUpOptions.MoveSpeed) //movespeed is not an instanced float right now
        {
            upgradeDescription.text = "Upgrades Movement Speed by 25%, from " + "5" + " to " + "6.25";
        }
    }
}
