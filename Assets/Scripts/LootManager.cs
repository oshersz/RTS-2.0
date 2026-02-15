using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager singleton { get; private set; }

    [SerializeField] GameObject moneyPrefab;
    [SerializeField] GameObject[] commonItems;
    [SerializeField] GameObject[] rareItems;
    [SerializeField] GameObject[] scarceItems;
    [SerializeField] GameObject[] nonexistentItems;

    private int moneyAmount;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        //FOR DEBUGGING, DELETE LATER
        if (Input.GetKeyDown(KeyCode.F8))
        {
            DropLoot(LootDrop.Common, CharacterStats.singleton.transform.position);
        }
    }
    /*
    *for (int i = 0;i<moneyAmount;i++)
    {
        Instantiate(moneyPrefab,transform.position,transform.rotation,null);
    }
    */
    public void DropLoot(LootDrop lootDrop, Vector3 creaturePosition)
    {
        Vector3 desiredLootPosition = new Vector3(creaturePosition.x, 0, creaturePosition.z);

        if (lootDrop == LootDrop.Common)
        {
            moneyAmount = Random.Range(0, 10);

            if (Random.Range(0,100)<70) //high chance for a common drop
            {
                Instantiate(commonItems[Random.Range(0,commonItems.Length)], desiredLootPosition, transform.rotation, null); 
            }
            if (Random.Range(0, 100) < 30) //lower chance of a rare drop
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length )], desiredLootPosition, transform.rotation, null); // commonItems.Length-1?
            }
        }

        if (lootDrop == LootDrop.Rare)
        {
            moneyAmount = Random.Range(10, 25);

            if (Random.Range(0, 100) < 55) //high chance for a rare drop
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 40) //good chance to get at least one common
            {
                Instantiate(commonItems[Random.Range(0, commonItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 40) 
            {
                Instantiate(commonItems[Random.Range(0, commonItems.Length)], desiredLootPosition, transform.rotation, null);
            }
        }

        if (lootDrop == LootDrop.Scarce)
        {
            moneyAmount = Random.Range(50, 150);

            if (Random.Range(0, 100) < 35) //reasonable chance for a scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 100) //at least one guranteed rare
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 25)
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
            }
        }

        if (lootDrop == LootDrop.Nonexistent)
        {
            moneyAmount = Random.Range(500, 2000);

            if (Random.Range(0, 100) < 60) //high  chance for a scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 20) //small  chance for an additional scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 100) //at least two guranteed rares
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
            }
            if (Random.Range(0, 100) < 40)
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, null);
            }

        }
        if (moneyAmount!=0)
        {
            GameObject tempCoinDrop = Instantiate(moneyPrefab, desiredLootPosition, transform.rotation, null);
            tempCoinDrop.name = moneyAmount + "coins"; // move when parsec performes better !
        }
    }
}
