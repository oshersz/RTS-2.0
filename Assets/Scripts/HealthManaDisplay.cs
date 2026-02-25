using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HealthManaDisplay : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI textDisplay;
    private bool active;
    public enum HealthManaExp
    {
        Health,
        Mana,
        Exp
    }
    public HealthManaExp typeOfTextDisplay;

    private void Update()
    {
        if (active)
        {
            if (typeOfTextDisplay == HealthManaExp.Health)
            {
                textDisplay.text = "Health: " + CharacterStats.singleton.currentHealth.ToString("F2") + "/" + CharacterStats.singleton.maxHealth;
            }
            else if (typeOfTextDisplay == HealthManaExp.Mana)
            {
                textDisplay.text = "Mana: " + CharacterStats.singleton.currentMana.ToString("F2") + "/" + CharacterStats.singleton.maxMana;
            }
            else if (typeOfTextDisplay == HealthManaExp.Exp)
            {
                textDisplay.text = "Exp: " + CharacterStats.singleton.currentExp + "/" + CharacterStats.singleton.expNeededForNextLevel;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        textDisplay.gameObject.SetActive(true);
        active = true;
    }
    

    public void OnPointerExit(PointerEventData eventdata)
    {
        active = false;
        textDisplay.gameObject.SetActive(false);
    }
}
