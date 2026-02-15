using UnityEngine;

public class MoveInCameraDirection : MonoBehaviour
{
    [SerializeField] GameObject targetingArrow;
    // Update is called once per frame
    void Update()
    {
        targetingArrow.transform.position = transform.position + Camera.main.transform.up * 2.25f;
        /*
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Camera.main.transform.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Camera.main.transform.up;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Camera.main.transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Camera.main.transform.right;
        }
        */
    }
}
