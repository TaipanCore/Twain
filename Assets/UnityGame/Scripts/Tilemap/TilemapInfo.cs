using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInfo : MonoBehaviour
{
    [HideInInspector] public Dictionary <Vector3Int, TileBase> allTiles = new Dictionary<Vector3Int, TileBase>();
    private void Awake()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt tilemapBounds = tilemap.cellBounds;
        foreach (Vector3Int pos in tilemapBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                allTiles.Add(pos, tile);              
            }
        }
    }
}
