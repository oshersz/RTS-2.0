using UnityEngine;

public class CharacterMoveInteract : MonoBehaviour
{
    [SerializeField] LayerMask interactMask;
    [SerializeField] float interactRadius;
    [SerializeField] float moveSpeed;
    private Collider[] raycastHits;
    public Vector2 moveDir;
    void Start()
    {
        
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !TextBox.singleton.IsDialogueActive())
        {
            if (Input.GetKey(KeyCode.W))
                moveDir += Vector2.up;

            if (Input.GetKey(KeyCode.S))
                moveDir -= Vector2.up;

            if (Input.GetKey(KeyCode.A))
                moveDir -= Vector2.right;

            if (Input.GetKey(KeyCode.D))
                moveDir += Vector2.right;

            transform.Translate(new Vector3(moveDir.x, 0, moveDir.y).normalized * moveSpeed * Time.deltaTime);
            moveDir = Vector2.zero;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            raycastHits = Physics.OverlapSphere(transform.position, interactRadius, interactMask);

            if (raycastHits.Length>0)
            {
                if (TextBox.singleton.IsDialogueActive())
                    TextBox.singleton.SkipInteraction();//raycastHits[0].GetComponent<Interactable>().SkipInteraction();
                else
                    raycastHits[0].GetComponent<Interactable>().Interact();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(transform.position, interactRadius);
    }
}
