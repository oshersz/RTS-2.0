using UnityEngine;

public class MouseHover : MonoBehaviour
{
    private Ray raycastFromMouse;
    private RaycastHit raycastHit;
    public LayerMask hoverMask;
    public Material outlineMaterial;

    private Transform lastObjectTargeted;

    void Update()
    {
        raycastFromMouse = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if (Physics.BoxCast(raycastFromMouse.origin, Vector3.one * 0.5f, raycastFromMouse.direction, out raycastHit, Quaternion.identity, 500, hoverMask))
        {
            ApplyOutline(raycastHit.transform);
        }
    }

    private void ApplyOutline(Transform objectToOutline)
    {
        if (objectToOutline!= lastObjectTargeted)
        {
            //unoptimized way
            outlineMaterial.SetColor("_Color", Color.black);
            Material[] materialArray = new Material[2];
            materialArray[1] = outlineMaterial;

            if (objectToOutline.GetComponent<Renderer>()!=null)
            {
                materialArray[0] = objectToOutline.GetComponent<Renderer>().material;
                objectToOutline.GetComponent<Renderer>().materials = materialArray;
            }
            else
            {
                for (int i=0;i<objectToOutline.childCount;i++)
                {
                    RecursiveOutline(objectToOutline.GetChild(i), materialArray);
                }
            }

            if (lastObjectTargeted!=null)
            {
                RemoveOutlineRecursive(lastObjectTargeted);
            }
        }
        lastObjectTargeted = objectToOutline;
    }

    public void ApplyOutline(Transform objectToOutline,Color outlineColor)
    {
        outlineMaterial.SetColor("_Color", outlineColor);
        Material[] materialArray = new Material[2];
        materialArray[1] = outlineMaterial;

        if (objectToOutline.GetComponent<Renderer>() != null)
        {
            materialArray[0] = objectToOutline.GetComponent<Renderer>().material;
            objectToOutline.GetComponent<Renderer>().materials = materialArray;
        }
        else
        {
            for (int i = 0; i < objectToOutline.childCount; i++)
            {
                RecursiveOutline(objectToOutline.GetChild(i), materialArray);
            }
        }
    }

    private void RecursiveOutline(Transform child,Material[] materialArray)
    {
        //Debug.Log("fuck");
        if (child.GetComponent<Renderer>()!= null)
        {
            materialArray[0] = child.GetComponent<Renderer>().material;
            child.GetComponent<Renderer>().materials = materialArray;
        }
        if (child.childCount == 0)
        {
            return;
        }
        else
        {
            for (int i=0;i<child.childCount;i++)
            {
                RecursiveOutline(child.GetChild(i),materialArray);
            }
        }
    }

    private void RemoveOutlineRecursive(Transform objectToRemoveOutline)
    {
        if (objectToRemoveOutline.GetComponent<Renderer>() != null)
        {
            Material[] materialArray = new Material[1];

            materialArray[0] = objectToRemoveOutline.GetComponent<Renderer>().materials[0];

            objectToRemoveOutline.GetComponent<Renderer>().materials = materialArray;
        }
        if (objectToRemoveOutline.childCount == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < objectToRemoveOutline.childCount; i++)
            {
                RemoveOutlineRecursive(objectToRemoveOutline.GetChild(i));
            }
        }




    }
}
