using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item itemDrop;

    private void Awake()
    {
        Destroy(gameObject, 20);
    }
}
