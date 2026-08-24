using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager singleton { get; private set; }

    [SerializeField] Transform lootParent;
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

        float chanceInc = CharacterStats.singleton.characterStats[(int)Stats.LootChanceIncrease];

        if (lootDrop == LootDrop.Common)
        {
            moneyAmount = Random.Range(0, 10);


            if (Random.Range(0,100)<70) //high chance for a common drop
            {
                Instantiate(commonItems[Random.Range(0,commonItems.Length)], desiredLootPosition, transform.rotation, lootParent); 
            }
            if (Random.Range(0, 100) < 30 + chanceInc) //lower chance of a rare drop
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length )], desiredLootPosition, transform.rotation, lootParent); // commonItems.Length-1?
            }
        }

        if (lootDrop == LootDrop.Rare)
        {
            moneyAmount = Random.Range(10, 25);

            if (Random.Range(0, 100) < 55 + chanceInc) //high chance for a rare drop
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 40) //good chance to get at least one common
            {
                Instantiate(commonItems[Random.Range(0, commonItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 40) 
            {
                Instantiate(commonItems[Random.Range(0, commonItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
        }

        if (lootDrop == LootDrop.Scarce)
        {
            moneyAmount = Random.Range(50, 150);

            if (Random.Range(0, 100) < 35 + chanceInc/2) //reasonable chance for a scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 100) //at least one guranteed rare
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 25 + chanceInc)
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
        }

        if (lootDrop == LootDrop.Nonexistent)
        {
            moneyAmount = Random.Range(500, 2000);

            if (Random.Range(0, 100) < 15 + chanceInc / 4) //high  chance for a scarce drop
            {
                Instantiate(nonexistentItems[Random.Range(0, nonexistentItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 60 + chanceInc/2) //high  chance for a scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 20 + chanceInc/2) //small  chance for an additional scarce drop
            {
                Instantiate(scarceItems[Random.Range(0, scarceItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 100) //at least two guranteed rares
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }
            if (Random.Range(0, 100) < 40 + chanceInc)
            {
                Instantiate(rareItems[Random.Range(0, rareItems.Length)], desiredLootPosition, transform.rotation, lootParent);
            }

        }
        if (moneyAmount!=0)
        {
            GameObject tempCoinDrop = Instantiate(moneyPrefab, desiredLootPosition, transform.rotation, lootParent);
            tempCoinDrop.name = moneyAmount + "coins"; // move when parsec performes better !
        }
    }
}
