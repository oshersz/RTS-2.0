using UnityEngine;

public class States
{
    public static State charState;
    public enum State
    {
        Idle,
        Moving,
        Chasing,
        Attacking
    }
}
