using UnityEngine;
using TMPro;
public class SoldEffect : MonoBehaviour
{
    public TextMeshProUGUI soldText;
    private Vector2 direction;
    void Start()
    {
        Destroy(gameObject, 1.5f);
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    void Update()
    {
        soldText.transform.Translate(direction * 100 * Time.deltaTime);
    }
}
