using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject equipButton;
    [SerializeField] GameObject sellButton;
    Item item;


    RectTransform UITransform;
    Vector3 positionForWindow;

    void Start()
    {
        UITransform = GetComponent<RectTransform>();
        equipButton.SetActive(false);
        sellButton.SetActive(false);
        item = GetComponent<ItemDrop>().itemDrop;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item.itemType == ItemType.Equipment)
        {
            equipButton.SetActive(true);

            if (transform.position.x < Screen.width / 2)
            {
                positionForWindow = transform.position - (Vector3.right * UITransform.rect.width * 1.575f);
            }
            else
            {
                positionForWindow = transform.position + (Vector3.right * UITransform.rect.width * 1.575f);
            }
            Inventory.singleton.CompareEquipment((Equipment)item);
            UIManager.singleton.ShowEquipmentPopup((Equipment)item,positionForWindow);
        }
        sellButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        equipButton.SetActive(false);
        sellButton.SetActive(false);

        UIManager.singleton.HideEquipmentPopup();
        UIManager.singleton.UpdateCharacterStats(); //to remove the stat comparison
    }

    public void Sell()
    {
        Inventory.singleton.SellItem(item);
    }

    public void Equip() //Button appears only for equipment
    {
        
        Inventory.singleton.EquipEquipmentItem((Equipment)item);
    }
}
