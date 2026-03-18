using UnityEngine;


public enum RangeIndicatorType
{
    characterDirectional,
    characterStatic,
    worldPos
}
public class RangeIndicator : MonoBehaviour
{
    [SerializeField] GameObject character;

    [SerializeField] LayerMask floorMask;
    [SerializeField] RangeIndicatorType rangeIndicatorType;
    private Ray raycastFromMouse;
    private RaycastHit raycastHit;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask))
        {
        }
        if (rangeIndicatorType == RangeIndicatorType.characterDirectional)
        {
            raycastHit.point = new Vector3(raycastHit.point.x, character.transform.position.y, raycastHit.point.z);
            transform.position = character.transform.position;
            transform.LookAt(raycastHit.point);
        }
        else if (rangeIndicatorType == RangeIndicatorType.worldPos)
        {
            transform.position = raycastHit.point;
        }
        else if (rangeIndicatorType == RangeIndicatorType.characterStatic)
        {
            transform.position =  new Vector3(character.transform.position.x,transform.position.y,character.transform.position.z);
        }

    }
}
