using UnityEngine;

public class SpawnMonsters : MonoBehaviour
{
    public static SpawnMonsters singleton { get; private set; }

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyPrefab2;
    [SerializeField] GameObject strongEnemyPrefab;
    [SerializeField] Transform spawnParent;
    public float spawnCooldown;
    private float spawnTimer;
    private Vector3 spawnLocation;

    private Quaternion startingRot;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        spawnTimer = Time.time + spawnCooldown;

        startingRot = transform.rotation;
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
            if (OneMinus() == 1)
                spawnLocation = new Vector3(Random.Range(-transform.localScale.x * 0.75f, transform.localScale.x * 0.75f), 1, Random.Range(transform.localScale.z * 0.5f, transform.localScale.z * 0.75f) * OneMinus());
            else
                spawnLocation = new Vector3(Random.Range(transform.localScale.x * 0.5f, transform.localScale.x * 0.75f) * OneMinus(), 1, Random.Range(-transform.localScale.z * 0.75f, transform.localScale.z * 0.75f));

            if (Random.Range(0, 2) == 1)
                Instantiate(enemyPrefab, transform.position + spawnLocation, Quaternion.identity, spawnParent);
            else
                Instantiate(enemyPrefab2, transform.position + spawnLocation, Quaternion.identity, spawnParent);
            spawnTimer = Time.time + spawnCooldown;
        }
    }

    public void Spawn(Vector3 spawnPoint,Transform dungeonParent)
    {
        int spawnAmount = Random.Range(1, 3);
        for (int i = 0; i < spawnAmount; i++)
        {

            spawnLocation = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            spawnLocation += spawnPoint;
            //spawnLocation = spawnPoint;

            if (Random.Range(0, 2) == 1)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity, dungeonParent); //spawnParent
                enemy.GetComponent<Enemy>().maxDespawnTime = 120;
            }
            else
            {
                GameObject enemy = Instantiate(enemyPrefab2, spawnLocation, Quaternion.identity, dungeonParent); //spawnParent
                enemy.GetComponent<Enemy>().maxDespawnTime = 120;
            }
            //spawnTimer = Time.time + spawnCooldown;
        }
    }

    public void Spawn(Vector3 spawnPoint, Transform dungeonParent, bool spawnStrongEnemy)
    {
        if (spawnStrongEnemy)
        {
            spawnLocation = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

            spawnLocation += spawnPoint;

            GameObject bossEnemy = Instantiate(strongEnemyPrefab, spawnLocation, Quaternion.identity, dungeonParent); //spawnParent
            bossEnemy.GetComponent<Enemy>().maxDespawnTime = 240;
        }
    }


    public void Spawn(Vector3 spawnPoint)
    {
        spawnLocation = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        spawnLocation += spawnPoint;
        //spawnLocation = spawnPoint;

        if (Random.Range(0, 2) == 1)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity, spawnParent);
            enemy.GetComponent<Enemy>().maxDespawnTime = 120;
        }
        else
        {
            GameObject enemy = Instantiate(enemyPrefab2, spawnLocation, Quaternion.identity, spawnParent);
            enemy.GetComponent<Enemy>().maxDespawnTime = 120;
        }
    }

    int OneMinus() //Returns 1 or -1 randomly
    {
        int num = Random.Range(1, 3);
        if (num == 2)
            num = -1;
        return num;
    }

    private void LateUpdate()
    {
        transform.rotation = startingRot;
    }
}
