using UnityEngine;

public abstract class Spells : Attacks
{
    public Spell spellType;
    public float spellDelay;
    public float spellCooldown;
    public float spellRadius;
    public float manaCost;
    //public float damage;
    public enum Spell
    {
        FirstTarget,
        Targeted,
        Positional,
        Vectorial,
        SelfTarget
    }

}
