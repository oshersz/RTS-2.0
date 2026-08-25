using UnityEngine;
using System.Collections.Generic;

public class DungeonManager : MonoBehaviour
{
    public GameObject tilePrefab;
    private Vector3 spawnPos; //make it private it make it random later
    [SerializeField] GameObject portal;
    [SerializeField] GameObject interactable;

    [SerializeField] Transform player; // I wish I didn't need to do this
    [SerializeField] SpawnMonsters monsterSpawner; //this too
    public static GameObject lastActiveDungeon; //this too
    void Start()
    {
        //GenerateDungeon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            GenerateDungeon();
            //CharacterStats.singleton.transform.position = startingTile
        }
    }

    public void GenerateDungeon()
    {
        //oh no it's like computer sciences all over again
        GameObject parent = new GameObject();
        parent.name = "Generated Dungeon";

        lastActiveDungeon = parent;

        //int[,] dungeonMatrix = new int[10,10];
        DungeonTile[,] tileMatrix = new DungeonTile[10,10];
        Vector3 dungeonSpawnPos;

        //consider changing
        spawnPos = new Vector3 (Random.Range(-50, 50), 0, Random.Range(-50, 50));

        spawnPos.x = spawnPos.x > 0 ? Mathf.Clamp(spawnPos.x, 10, 50) : Mathf.Clamp(spawnPos.x, -50, -10);
        spawnPos.z = spawnPos.z > 0 ? Mathf.Clamp(spawnPos.z, 10, 50) : Mathf.Clamp(spawnPos.z, -50, -10);
        spawnPos *= 15;
        spawnPos.y = -0.5f; //platforms width are 1
        //spawnPos.x > 0 ? Debug.Log("a") : Debug.Log("b");


        parent.transform.position = spawnPos;

        //int columnLength = dungeonMatrix.GetLength(0);
        //int rowLength = dungeonMatrix.GetLength(1);

        int columnLength = tileMatrix.GetLength(0);
        int rowLength = tileMatrix.GetLength(1);

        for (int i = 0;i<columnLength;i++) //generating the matrix
        {
            for (int j = 0;j<rowLength;j++)
            {
                dungeonSpawnPos = new Vector3(spawnPos.x + i*10, spawnPos.y, spawnPos.z+j*10);
                GameObject dungeonTile = Instantiate(tilePrefab, dungeonSpawnPos, Quaternion.identity,parent.transform);
                tileMatrix[i,j] = dungeonTile.GetComponent<DungeonTile>();
                tileMatrix[i, j].tileLocation = new Vector2(i, j);
            }
        }

        for (int i = 0; i < columnLength; i++) //placing the walls
        {
            for (int j = 0; j < rowLength; j++)
            {
                if (j == 0 || j == columnLength - 1)
                {
                    tileMatrix[i,j].tileType = TileType.Wall;
                }
                if (i == 0 || i == rowLength - 1)
                {
                    tileMatrix[i, j].tileType = TileType.Wall;
                }
            }

        }

        Vector2 startingTile = new Vector2(Random.Range(1, 9), Random.Range(1, 9));
        tileMatrix[(int)startingTile.x, (int)startingTile.y].tileType = TileType.StartingTile;

        Vector3 startingTilePosition = new Vector3(spawnPos.x + startingTile.x * 10, 1, spawnPos.z + startingTile.y * 10);


        int numberOfUnconfigTiles = 63; //8 * 8 = 64 - 1 (the starting tile))
        List<DungeonTile> potentialNeighbours = new List<DungeonTile>();

        if (tileMatrix[(int)startingTile.x+1, (int)startingTile.y].tileType!= TileType.Wall) //right
        {
            potentialNeighbours.Add(tileMatrix[(int)startingTile.x + 1, (int)startingTile.y]);
            tileMatrix[(int)startingTile.x + 1, (int)startingTile.y].tileType = TileType.Playable;
        }
        if (tileMatrix[(int)startingTile.x - 1, (int)startingTile.y].tileType != TileType.Wall) //left
        {
            potentialNeighbours.Add(tileMatrix[(int)startingTile.x - 1, (int)startingTile.y]);
            tileMatrix[(int)startingTile.x - 1, (int)startingTile.y].tileType = TileType.Playable;
        }
        if (tileMatrix[(int)startingTile.x, (int)startingTile.y + 1].tileType != TileType.Wall) //up
        {
            potentialNeighbours.Add(tileMatrix[(int)startingTile.x , (int)startingTile.y + 1]);
            tileMatrix[(int)startingTile.x, (int)startingTile.y + 1].tileType = TileType.Playable;
        }
        if (tileMatrix[(int)startingTile.x, (int)startingTile.y - 1].tileType != TileType.Wall) //down
        {
            potentialNeighbours.Add(tileMatrix[(int)startingTile.x, (int)startingTile.y - 1]);
            tileMatrix[(int)startingTile.x, (int)startingTile.y - 1].tileType = TileType.Playable;
        }


        while (potentialNeighbours.Count>0)
        {
            int count = potentialNeighbours.Count;
            for(int i=0;i<count;i++)
            {
                DungeonTile tile = potentialNeighbours[i];

                int numberOfNeighbours = 4;
                
                if (tileMatrix[(int)tile.tileLocation.x + 1, (int)tile.tileLocation.y].tileType == TileType.Unconfigured) //right
                {
                    if (Random.Range(0, 75) > numberOfUnconfigTiles)
                    {
                        tileMatrix[(int)tile.tileLocation.x + 1, (int)tile.tileLocation.y].tileType = TileType.Empty;
                        numberOfNeighbours--;
                        numberOfUnconfigTiles--;
                        
                    }
                    else
                    {
                        potentialNeighbours.Add(tileMatrix[(int)tile.tileLocation.x + 1, (int)tile.tileLocation.y]);
                        tileMatrix[(int)tile.tileLocation.x + 1, (int)tile.tileLocation.y].tileType = TileType.Playable;
                        numberOfUnconfigTiles--;
                    }

                }
                else
                    numberOfNeighbours--;
                if (tileMatrix[(int)tile.tileLocation.x - 1, (int)tile.tileLocation.y].tileType == TileType.Unconfigured) //left
                {
                    if (Random.Range(0, 75) > numberOfUnconfigTiles)
                    {
                        tileMatrix[(int)tile.tileLocation.x - 1, (int)tile.tileLocation.y].tileType = TileType.Empty;
                        numberOfNeighbours--;
                        numberOfUnconfigTiles--;
                    }
                    else
                    {
                        potentialNeighbours.Add(tileMatrix[(int)tile.tileLocation.x - 1, (int)tile.tileLocation.y]);
                        tileMatrix[(int)tile.tileLocation.x - 1, (int)tile.tileLocation.y].tileType = TileType.Playable;
                        numberOfUnconfigTiles--;
                    }
                }
                else
                    numberOfNeighbours--;
                if (tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y + 1].tileType == TileType.Unconfigured) //up
                {
                    if (Random.Range(0, 75) > numberOfUnconfigTiles)
                    {
                        tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y + 1].tileType = TileType.Empty;
                        numberOfNeighbours--;
                        numberOfUnconfigTiles--;
                    }
                    else
                    {
                        potentialNeighbours.Add(tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y + 1]);
                        tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y + 1].tileType = TileType.Playable;
                        numberOfUnconfigTiles--;
                    }
                }
                else
                    numberOfNeighbours--;
                if (tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y - 1].tileType == TileType.Unconfigured) //down
                {
                    if (Random.Range(0, 75) > numberOfUnconfigTiles)
                    {
                        tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y - 1].tileType = TileType.Empty;
                        numberOfNeighbours--;
                        numberOfUnconfigTiles--;
                    }
                    else
                    {
                        potentialNeighbours.Add(tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y - 1]);
                        tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y - 1].tileType = TileType.Playable;
                        numberOfUnconfigTiles--;
                    }
                }
                else
                    numberOfNeighbours--;

                if (numberOfNeighbours <= 0)
                {
                    if (potentialNeighbours.Count == 1) //last tile contains the treasure
                    {
                        tileMatrix[(int)tile.tileLocation.x, (int)tile.tileLocation.y].tileType = TileType.Treasure;
                    }
                    potentialNeighbours.Remove(tile);
                    count--;
                }
                
            }
        }

        //CharacterStats.singleton.transform.position = startingTilePosition;
        player.GetComponent<CharacterController>().enabled = false;
        Vector3 returnPos = player.position;
        player.position = startingTilePosition;
        player.GetComponent<CharacterController>().enabled = true;

        UIManager.singleton.TeleportAllies(startingTilePosition);

        //change this later
        for (int i = 0; i < columnLength; i++) //generating the matrix
        {
            for (int j = 0; j < rowLength; j++)
            {
                if (tileMatrix[i,j].tileType == TileType.Playable)
                {
                    dungeonSpawnPos = new Vector3(spawnPos.x + i * 10, 1.08f, spawnPos.z + j * 10);

                    if (Random.Range(0,4) == 0)
                    {
                        monsterSpawner.Spawn(dungeonSpawnPos, parent.transform);
                    }

                    int interactablesOnTile = Random.Range(0, 3);
                    for (int d = 0;d<interactablesOnTile;d++)
                    {
                        Vector3 spawnLocation = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

                        Instantiate(interactable, dungeonSpawnPos + spawnLocation, Quaternion.identity, parent.transform); //adding the interactables into the dungeon
                    }


                }
                if (tileMatrix[i, j].tileType == TileType.Treasure)
                {
                    dungeonSpawnPos = new Vector3(spawnPos.x + i * 10, 0, spawnPos.z + j * 10);
                    LootManager.singleton.DropLoot(LootDrop.Rare, dungeonSpawnPos);

                    GameObject backPortal = Instantiate(portal, dungeonSpawnPos, Quaternion.identity, parent.transform);
                    backPortal.GetComponent<BackPortal>().returnPos = returnPos;

                    monsterSpawner.Spawn(dungeonSpawnPos, parent.transform,true); //spawning the boss
                }
            }
        }
    }

    public static void DestroyDungeon()
    {
        Destroy(lastActiveDungeon);
    }
}
