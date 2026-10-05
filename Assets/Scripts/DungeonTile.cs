using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    [HideInInspector] public TileType tileType;
    [HideInInspector] public Vector2 tileLocation;
    [SerializeField] GameObject wallTileGraphic;
    [SerializeField] GameObject startingTileGraphic;
    [SerializeField] GameObject floorTileGraphic;
    [SerializeField] GameObject treasureTileGraphic;
    [SerializeField] GameObject emptyTileGraphic;
    void Start()
    {
        if (tileType == TileType.Wall)
        {
            wallTileGraphic.SetActive(true);
        }
        if (tileType == TileType.StartingTile)
        {
            startingTileGraphic.SetActive(true);
        }
        else if (tileType == TileType.Playable)
        {
            floorTileGraphic.SetActive(true);
        }
        else if (tileType == TileType.Treasure)
        {
            treasureTileGraphic.SetActive(true);
        }
        else if (tileType == TileType.Empty)
        {
            emptyTileGraphic.SetActive(true);
        }
    }
}

public enum TileType
{
    Unconfigured,
    StartingTile,
    Wall,
    Playable,
    Treasure,
    Empty
}
