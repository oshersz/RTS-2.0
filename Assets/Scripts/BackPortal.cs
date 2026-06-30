using UnityEngine;

public class BackPortal : MonoBehaviour
{
    [HideInInspector] public Vector3 returnPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Player"))
        {
            Debug.Log("portal working");
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.position = returnPos;
            other.GetComponent<CharacterController>().enabled = true;
        }
    }

}
