using UnityEngine;

public class BackPortal : MonoBehaviour
{
    [HideInInspector] public Vector3 returnPos;

    private void Update()
    {
        if ((int)(Time.time)%2 == 0)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("portal working");
            other.GetComponent<CharacterController>().enabled = false;
            other.transform.position = returnPos;
            other.GetComponent<CharacterController>().enabled = true;
        }
    }

}
