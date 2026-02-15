using UnityEngine;

public class ChildrenPicker : MonoBehaviour
{
    [SerializeField] int amountOfChildrenToPick;

    public void OnEnable()
    {
        int[] childrenToPick = new int[transform.childCount];
        for (int i = 0; i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            childrenToPick[i] = i;
        }
        //smh thought there would be an easy way to shuffle, taken from google/unity forums/wikipedia
        for (int t = 0; t < childrenToPick.Length; t++)
        {
            int num = childrenToPick[t];
            int random = Random.Range(t, childrenToPick.Length);
            childrenToPick[t] = childrenToPick[random];
            childrenToPick[random] = num;
        }

        //this loop is mine ->
        for (int i =0;i<amountOfChildrenToPick;i++)
        {
            transform.GetChild(childrenToPick[i]).gameObject.SetActive(true);
        }
    }
}
