using System.Collections.Generic;
using UnityEngine;

public class InteractableDestructible : Interactable
{
    //[SerializeField] Rigidbody[] flyingMeshes;
    [SerializeField] List<Rigidbody> flyingMeshes;
    //private MeshRenderer MR;
    //private Collider collide;
    public override void Interact()
    {
        Destroy(gameObject, 5);
        GetComponent<MeshRenderer>().enabled = false; //not showing the current graphics
        GetComponent<Collider>().enabled = false;
        if (Random.Range(0,3) == 0)
        {
            LootManager.singleton.DropLoot(LootDrop.Common, transform.position);
        }

        gameObject.layer = LayerMask.GetMask("Default"); // making sure object is not interacted again

        int numberOfMeshes = Random.Range(1, flyingMeshes.Count); //picking a random number of flying parts
        int meshIndex;
        Vector3 explosionForce;
        for (int i=0;i<numberOfMeshes;i++)
        {
            meshIndex = Random.Range(0, flyingMeshes.Count);
            explosionForce = new Vector3(Random.Range(-5, 5), Random.Range(5, 10), Random.Range(-5, 5));
            flyingMeshes[meshIndex].gameObject.SetActive(true);
            flyingMeshes[meshIndex].AddForce(explosionForce,ForceMode.Impulse);
            flyingMeshes.RemoveAt(meshIndex);
        }
    }
}
