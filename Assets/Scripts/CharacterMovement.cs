using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private LayerMask floorMask;
    private CharacterController CC;
    //[Range(0,20)] [SerializeField] float moveSpeed; //movespeed is taken from character stats instead
    private Vector3 moveDirection;

    private float rollValue;
    private float rollTimer;
    private float rollCD;
    private bool rolling;

    private Ray raycastFromMouse;
    private RaycastHit raycastHit;

    private Transform followTarget;
    private Vector3 followTargetPosition;
    private bool following = false;
    private float attackRange;

    private Interactable lastInteractable;

    private float rotationSpeed = 700;

    private Quaternion desiredRotation;

    public static State charState;

    public State inspectorState;

    public Vector3 raycasthitpoint;

    struct Equipment
    {
        public GameObject prefab;
        public bool found;
    }

    void Start()
    {
        CC = GetComponent<CharacterController>();
        floorMask = 256;
    }

    void Update()
    {
        raycasthitpoint = raycastHit.point;
        /*
        Debug.Log(charState.ToString());

        if (Input.GetKeyDown(KeyCode.Space) && !rolling && Time.time > rollCD)
        {
            StartRoll();
        }
        if (rolling)
        {
            Roll();
            if (rollTimer >= 1)
                rolling = false;
        }
        else
        */
        Move();
    }

    private void Move()
    {
        if (Input.GetMouseButton(1)) //movement with right mouse click
        {
            raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //because it's a raycast, it hits invisble colliders
            if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask))
            {
            }
            raycastHit.point = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z); //preventing the char from unwanted rotation;

            moveDirection = raycastHit.point - transform.position;

            if (charState != State.AttackStun)
                desiredRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            // unfollow
            following = false;
            followTarget = null;

        }
        if (Input.GetMouseButtonDown(1)) //movement with right mouse click
        {
            /**
            raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //because it's a raycast, it hits invisble colliders
            if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask))
            {
            }
            raycastHit.point = new Vector3(raycastHit.point.x, transform.position.y, raycastHit.point.z); //preventing the char from unwanted rotation;

            CharacterVisual.singleton.MoveVFX(new Vector3(raycastHit.point.x, 0.25f, raycastHit.point.z));

            moveDirection = raycastHit.point - transform.position;

            if (charState != State.AttackStun)
                desiredRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            // unfollow
            following = false;
            followTarget = null;
            */
            CharacterVisual.singleton.MoveVFX(new Vector3(raycastHit.point.x, 0.25f, raycastHit.point.z));

        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && !rolling && Time.time > rollCD)
        {
            StartRoll();
        }

        if (charState != State.AttackStun)
        {
            if (rolling)
            {
                charState = State.Rolling;
                Action();
            }
            else if (following && Vector3.Distance(transform.position, followTargetPosition) > attackRange)
            {
                charState = State.Chasing;
                if (followTarget != null)
                    Action();
            }
            else if (following && Vector3.Distance(transform.position, followTargetPosition) < attackRange)
            {
                if (lastInteractable!= null)
                {
                    lastInteractable.Interact();
                    lastInteractable = null;
                }
                charState = State.Attacking;
                Action();
            }
            else if (Vector3.Distance(transform.position, raycastHit.point) > 1)
            {
                charState = State.Moving;
                Action();
            }
            else
            {
                charState = State.Idle;
            }
        }

        inspectorState = charState;
    }

    public void FollowTarget(Transform target, float attackRange)
    {
        lastInteractable = null;
        if (target != null)
        {

            following = true;
            followTarget = target;
            followTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z); //preventing the char from unwanted rotation;

            this.attackRange = attackRange;
            charState = State.Chasing;
            Action();
            //Action(State.Chasing);
        }
        else
        {
            following = false;
            followTarget = null;

            if (Vector3.Distance(transform.position, raycastHit.point) < 1)
            {
                //charState = State.Idle;
                raycastHit.point = transform.position; //consider changing;
            }
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public void FollowTarget(Transform target, float attackRange,Interactable interactable)
    {
        lastInteractable = interactable;
        following = true;
        followTarget = target;
        followTargetPosition = new Vector3(target.position.x, transform.position.y, target.position.z); //preventing the char from unwanted rotation;

        this.attackRange = attackRange;
        charState = State.Chasing;
        Action();
    }

    private void Action()
    {
        if (charState == State.Chasing)
        {
            followTargetPosition = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z); //preventing the char from unwanted rotation;

            moveDirection = followTargetPosition - transform.position;

            desiredRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);

            CC.Move(moveDirection.normalized * CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] * Time.deltaTime);
        }
        else if (charState == State.Attacking)
        {
            if (followTarget == null)
            {
                following = false;
                raycastHit.point = transform.position;
                //charState = State.Idle; //wtf is going on
                return;
            }
            followTargetPosition = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z); //preventing the char from unwanted rotation;

            moveDirection = followTargetPosition - transform.position;

            desiredRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
        }
        else if (charState == State.Rolling)
        {
            rollTimer += Time.deltaTime * 1.5f; //roll duration is 2/3 second
            if (rollTimer>=1)
            {
                rolling = false;
                raycastHit.point = transform.position; //prevents character from moving after rolling
                charState = State.Idle;
                //transform.Rotate(-90, 0, 0);
                return;
            }
            //rollValue = easeOutQuint(1-rollTimer);
            rollValue = easeInCubic(1-rollTimer);
            //rollValue = easeOutCubic(rollTimer);
            //Debug.Log(rollValue);
            CC.Move(moveDirection.normalized * CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] * rollValue * 10 * Time.deltaTime);
        }
        else if (charState == State.Moving)
        {
            CC.Move(moveDirection.normalized * CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] * Time.deltaTime);
            //CC.Move(transform.forward * moveSpeed * Time.deltaTime);
        }
    }

    private void StartRoll()
    {
        rolling = true;
        rollCD = Time.time + 3f;
        rollTimer = 0; //goes from 0 to 1 during the roll
        following = false;

        //effect
        transform.Rotate(90, 0, 0);
    }
    private void Roll()
    {
        rollTimer += Time.deltaTime * 3; //roll duration is 1/3 second
        rollValue = easeOutQuint(rollTimer);
        CC.Move(moveDirection.normalized * CharacterStats.singleton.characterStats[(int)Stats.MoveSpeed] * rollValue * 2 * Time.deltaTime);
    }


    public float easeOutQuint(float animationPer)
    {
            return 1 - Mathf.Pow(1 - animationPer, 5);
    }

    public float easeInOutSine(float animationPer)
    {
        return -(Mathf.Cos(Mathf.PI * animationPer) - 1) / 2;
    }
    public float easeInCubic(float animationPer)
    {
        return animationPer * animationPer * animationPer;
    }
    public float easeOutCubic(float animationPer)
    {
        return 1 - Mathf.Pow(1 - animationPer, 3);
    }
}




public enum State
{
    Idle,
    Moving,
    Chasing,
    Attacking,
    AttackStun,
    Rolling
}
