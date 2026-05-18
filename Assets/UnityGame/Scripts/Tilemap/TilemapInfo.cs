using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInfo : MonoBehaviour
{
    [HideInInspector] public Dictionary <Vector3Int, TileBase> allTiles = new Dictionary<Vector3Int, TileBase>();
    
    private Tilemap tilemap;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        UpdateAllTiles();
    }
    public void  UpdateAllTiles()
    {
        BoundsInt tilemapBounds = tilemap.cellBounds;
        foreach (Vector3Int pos in tilemapBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile)
            {
                allTiles.TryAdd(pos, tile);
            }
        }
    }
}
