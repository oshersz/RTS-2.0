using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject character;

    public float zoomAmount;
    private float desiredZoomAmount;
    private Vector3 desiredLocation;
    private Vector3 cameraPos;
    void Start()
    {
        cameraPos = Camera.main.transform.position;
        desiredZoomAmount = zoomAmount;
    }

    void Update()
    {
        if (Input.mouseScrollDelta!=Vector2.zero)
        {
            desiredZoomAmount += Input.mouseScrollDelta.y * 0.05f;
            //zoomAmount += Input.mouseScrollDelta.y * 0.05f;
            zoomAmount = Mathf.Clamp(zoomAmount, 0.375f,0.75f);
            desiredZoomAmount = Mathf.Clamp(desiredZoomAmount, 0.375f, 0.75f);
        }

        if (zoomAmount<desiredZoomAmount)
        {
            zoomAmount += Time.deltaTime*0.4f;
            if (zoomAmount>desiredZoomAmount)
            {
                zoomAmount = desiredZoomAmount;
            }
        }
        else if (zoomAmount>desiredZoomAmount)
        {
            zoomAmount -= Time.deltaTime * 0.4f;
            if (zoomAmount < desiredZoomAmount)
            {
                zoomAmount = desiredZoomAmount;
            }
        }
    }

    private void LateUpdate()
    {
        //transform.position = character.transform.position;
        desiredLocation = new Vector3(character.transform.position.x, 0, character.transform.position.z);
        //desiredLocation = new Vector3(character.transform.position.x- Camera.main.transform.position.x, 0, character.transform.position.z- Camera.main.transform.position.z);
        transform.position = Vector3.Lerp(Camera.main.transform.position, desiredLocation-cameraPos, zoomAmount);
    }
}
