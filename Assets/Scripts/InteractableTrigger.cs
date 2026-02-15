using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : Interactable
{
    public bool interacted = false;

    public UnityEvent interactionEvent;
    public override void Interact()
    {
        interacted = true;
        interactionEvent.Invoke();
    }
}
