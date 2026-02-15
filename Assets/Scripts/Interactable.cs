using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");   
    }

    public abstract void Interact();

    //public abstract void SkipInteraction();
}
