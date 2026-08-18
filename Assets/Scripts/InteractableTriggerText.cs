using UnityEngine;
using UnityEngine.Events;

public class InteractableTriggerText : Interactable
{
    public bool interacted = false;

    public UnityEvent interactionEvent;

    [SerializeField] InteractableTrigger gameObjectNeededForInteraction;
    public DialogueText[] dialogueTexts;
    private int[] priorities;

    private int indexToRemember;
    private int priorityToRemember;

    private void Start()
    {
        priorities = new int[dialogueTexts.Length];
        for (int i = 0; i < dialogueTexts.Length; i++)
        {
            priorities[i] = dialogueTexts[i].priority; //otherwise it overwrites the original file asset
        }
    }

    public override void Interact()
    {
        interacted = true;
        interactionEvent.Invoke();

        indexToRemember = 0;
        priorityToRemember = -1;
        for (int i = 0; i < dialogueTexts.Length; i++)
        {
            if (priorities[i] > priorityToRemember)
            {
                if (dialogueTexts[i].conditionForDialogue == DialogueText.Conditions.NeedsGameObjectInteraction && !gameObjectNeededForInteraction.interacted)
                {

                }
                else
                {
                    priorityToRemember = priorities[i];
                    indexToRemember = i;
                }
            }
        }
        if (dialogueTexts[indexToRemember].conditionForDialogue == DialogueText.Conditions.NeedsGameObjectInteraction && gameObjectNeededForInteraction.interacted) //lord have mercy
        {
            TextBox.singleton.DisplayTexts(dialogueTexts[indexToRemember].dialogueTexts);
        }
        else if (dialogueTexts[indexToRemember].conditionForDialogue == DialogueText.Conditions.HasNextDialogue)
        {
            TextBox.singleton.DisplayTexts(dialogueTexts[indexToRemember].dialogueTexts);
            priorities[indexToRemember] = -1;
        }
        else
        {
            TextBox.singleton.DisplayTexts(dialogueTexts[indexToRemember].dialogueTexts); //0
        }

        Destroy(this);
        //this.enabled = false;

    }
}
