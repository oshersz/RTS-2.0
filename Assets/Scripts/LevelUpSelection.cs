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
        Gold,
        Exp,
        Dodge
    }
    public LevelUpOptions levelUpOptions;

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
        else if (levelUpOptions == LevelUpOptions.MoveSpeed)
        {
            upgradeDescription.text = "Upgrades Movement Speed by 25%, from " + CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] + " to " + CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] * 1.25f;
        }
        else if (levelUpOptions == LevelUpOptions.Gold)
        {
            upgradeDescription.text = "Upgrades Gold Gained by 25%, from " + CharacterStats.singleton.characterStats[(int)Stats.GoldIncrease] + " to " + (CharacterStats.singleton.characterStats[(int)Stats.GoldIncrease] +25) + "%";
        }
        else if (levelUpOptions == LevelUpOptions.Exp)
        {
            upgradeDescription.text = "Upgrades Exp Gained by 20%, from " + CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease] + " to " + (CharacterStats.singleton.characterStats[(int)Stats.ExpIncrease] + 20) + "%";
        }
        else if (levelUpOptions == LevelUpOptions.Dodge)
        {
            upgradeDescription.text = "Upgrades Dodge Chance by 10%, from " + CharacterStats.singleton.characterStats[(int)Stats.DodgeChance] + " to " + (CharacterStats.singleton.characterStats[(int)Stats.DodgeChance] + 10) + "%";
        }
    }

    public void LevelUp()
    {
        CharacterStats.singleton.LevelUpSelect(levelUpOptions);
    }
}
