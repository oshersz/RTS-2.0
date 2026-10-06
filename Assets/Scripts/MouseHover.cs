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
        if (objectToOutline != lastObjectTargeted)
        {
            outlineMaterial.SetColor("_Color", Color.black);

            Material[] materialArray = new Material[2];
            materialArray[1] = outlineMaterial;

            Renderer[] renderers;

            renderers = objectToOutline.GetComponentsInChildren<Renderer>();

            for (int i=0;i<renderers.Length;i++)
            {
                materialArray[0] = renderers[i].material;
                renderers[i].materials = materialArray;
            }

            if (lastObjectTargeted != null)
            {
                RemoveOutline(lastObjectTargeted);
            }
        }
        lastObjectTargeted = objectToOutline;
    }

    public void ApplyOutline(Transform objectToOutline,Color outlineColor)
    {
        outlineMaterial.SetColor("_Color", outlineColor);

        Material[] materialArray = new Material[2];
        materialArray[1] = outlineMaterial;

        Renderer[] renderers;

        renderers = objectToOutline.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            materialArray[0] = renderers[i].material;
            renderers[i].materials = materialArray;
        }
    }

    private void RemoveOutline(Transform objectToRemoveOutline)
    {
        Renderer[] renderers;

        renderers = objectToRemoveOutline.GetComponentsInChildren<Renderer>();

        Material[] materialArray = new Material[1];

        for (int i = 0; i < renderers.Length; i++)
        {
            materialArray[0] = renderers[i].materials[0];
            renderers[i].materials = materialArray;
        }

    }

}
