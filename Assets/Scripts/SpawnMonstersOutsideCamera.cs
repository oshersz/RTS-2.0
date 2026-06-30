using UnityEngine;

public class SpawnMonstersOutsideCamera : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyPrefab2;
    [SerializeField] Transform spawnParent;
    public float spawnCooldown;
    private float spawnTimer;
    private Vector3 spawnLocation;

    public Vector3 adjustSpawnPos;

    void Start()
    {
        spawnTimer = Time.time + spawnCooldown;
    }

    void Update()
    {
        if (Time.time > spawnTimer)
        {
            Spawn();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        int spawnAmount = Random.Range(1, 6);
        for (int i = 0; i < spawnAmount; i++)
        {
            //spawnLocation = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0.5f, 10f));
            spawnLocation = Camera.main.ViewportToWorldPoint(adjustSpawnPos);
            /*
            if (OneMinus() == 1)
                spawnLocation = new Vector3(Random.Range(-transform.localScale.x * 0.75f, transform.localScale.x * 0.75f), 1, Random.Range(transform.localScale.z * 0.5f, transform.localScale.z * 0.75f) * OneMinus());
            else
                spawnLocation = new Vector3(Random.Range(transform.localScale.x * 0.5f, transform.localScale.x * 0.75f) * OneMinus(), 1, Random.Range(-transform.localScale.z * 0.75f, transform.localScale.z * 0.75f));
            */
            if (Random.Range(0, 2) == 1)
                Instantiate(enemyPrefab, transform.position + spawnLocation, Quaternion.identity, spawnParent);
            else
                Instantiate(enemyPrefab2, transform.position + spawnLocation, Quaternion.identity, spawnParent);
            spawnTimer = Time.time + spawnCooldown;
        }
    }

    int OneMinus() //Returns 1 or -1 randomly
    {
        int num = Random.Range(1, 3);
        if (num == 2)
            num = -1;
        return num;
    }
}
