using UnityEngine;
using System;
[CreateAssetMenu(fileName = "DialogueText", menuName = "Scriptable Objects/DialogueText")]
public class DialogueText : ScriptableObject
{
    public enum Conditions
    {
        Nothing,
        HasNextDialogue,//NeedsPreviousDialogue,
        NeedsGameObjectInteraction,
        NeedsGameObjectActivated
    }

    [Serializable]
    public struct Dialogue
    {
        //public bool needsConditionMet;
        //[SerializeField] GameObject gameObjectForCondition;
        //public Conditions[] conditionRequired;
        [TextArea] public string text;
        public TextBox.TextEffects playerExpression;
        public TextBox.TextEffects interactableExpression;
    }
    public int priority;
    public Conditions conditionForDialogue;
    //public DialogueText previousDialogueForCondition;
    //public bool dialogueRead;
    //[SerializeField] GameObject gameObjectThatNeedsInteraction;
    public Dialogue[] dialogueTexts;
    
}
