using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject character;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = character.transform.position;
    }
}
