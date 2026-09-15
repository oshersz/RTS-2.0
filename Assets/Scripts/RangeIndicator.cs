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

    private bool usingExternalPoint;
    private float spellRadius;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(raycastFromMouse, out raycastHit, 500, floorMask))
        {
            if (usingExternalPoint)
            {
                Vector3 spellDirection = new Vector3(raycastHit.point.x - character.transform.position.x, 0, raycastHit.point.z - character.transform.position.z); // making sure we don't get rotation on the X axis
                spellDirection.Normalize();
                raycastHit.point = character.transform.position + spellDirection * spellRadius;
            }
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

    public void SetRaycastHit(float newSpellRadius)
    {
        usingExternalPoint = true;
        spellRadius = newSpellRadius;
    }
}
