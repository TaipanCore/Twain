using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;
using UnityEngine.AI;

public class TilemapBordersCreator : MonoBehaviour
{
    [SerializeField] private Tile BorderTile;
    [SerializeField] private TilemapInfo TilemapInfo;
    [SerializeField] private NavMeshSurface navMesh;
    private void Start()
    {
        Tilemap borderTilemap = GetComponent<Tilemap>();
        Tilemap groundTilemap = TilemapInfo.GetComponent<Tilemap>();
        foreach (KeyValuePair<Vector3Int, TileBase> tilePair in TilemapInfo.allTiles)
        {
            Vector3Int start = tilePair.Key + new Vector3Int(-1,-1, 0);
            BoundsInt bounds = new BoundsInt(start, new Vector3Int(3, 3, 1));
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (groundTilemap.GetTile(pos) == null)
                {
                    borderTilemap.SetTile(pos, BorderTile);
                }
            }
        }
        navMesh.BuildNavMesh();
    }
}
