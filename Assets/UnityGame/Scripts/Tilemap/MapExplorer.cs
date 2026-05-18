using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapExplorer : MonoBehaviour
{
    [SerializeField] private TilemapInfo groundTilemapInfo;
    [SerializeField] private TileBase exploredTile;
    [SerializeField] private float delayBetweenMapUpdates;
    private WaitForSeconds timerBetweenMapUpdates;
    [Header("Explorer colliders")]
    [SerializeField] private Collider2D[] whenInLightSideColliders;
    [SerializeField] private Collider2D[] whenInDarkSideColliders;
    [SerializeField] private Collider2D[] whenInEquilibriumColliders;
    
    
    private Tilemap mapTilemap;
    private TilemapInfo mapTilemapInfo;
    private Coroutine mapExplorerCoroutine;

    private void Start()
    {
        mapTilemap = GetComponent<Tilemap>();
        mapTilemapInfo = GetComponent<TilemapInfo>();
        timerBetweenMapUpdates = new WaitForSeconds(delayBetweenMapUpdates);
        BuildUnexploredMap();
        StartMapExploration();
    }

    public void StartMapExploration()
    {
        mapExplorerCoroutine ??= StartCoroutine(MapExplorerCoroutine());
    }

    public void StopMapExploration()
    {
        if (mapExplorerCoroutine != null)
            StopCoroutine(mapExplorerCoroutine);
    }

    private void BuildUnexploredMap()
    {
        foreach (var tile in groundTilemapInfo.allTiles)
        {
            mapTilemap.SetTile(tile.Key, exploredTile);
            mapTilemap.SetColor(tile.Key, Color.black);
        }
        mapTilemapInfo.UpdateAllTiles();
    }
    
    private IEnumerator MapExplorerCoroutine()
    {
        while (true)
        {
            switch (GameManager.currentCharacter)
            {
                case var obj when obj == GameManager.lightSide:
                    ExploreMap(whenInLightSideColliders);
                    break;
                case var obj when obj == GameManager.darkSide:
                    ExploreMap(whenInDarkSideColliders);
                    break;
                case var obj when obj == GameManager.equilibrium:
                    ExploreMap(whenInEquilibriumColliders);
                    break;
            }
            yield return timerBetweenMapUpdates;
        }
    }

    private void ExploreMap(Collider2D[] explorerColliders)
    {
        foreach (Collider2D expCollider in explorerColliders)
        {
            Vector3Int minBound = mapTilemap.WorldToCell(expCollider.bounds.min);
            Vector3Int maxBound = mapTilemap.WorldToCell(expCollider.bounds.max);
            for (int x = minBound.x; x <= maxBound.x; x++)
            {
                for (int y = minBound.y; y <= maxBound.y; y++)
                {
                    Vector3Int cellCoords = new Vector3Int(x, y, 0);
                    if (mapTilemap.HasTile(cellCoords) && mapTilemap.GetColor(cellCoords) == Color.black && expCollider.OverlapPoint(mapTilemap.GetCellCenterWorld(cellCoords)))
                    {
                        mapTilemap.SetColor(cellCoords, Color.white);
                    }
                }
            }
        }
    }
}
