using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapExplorer : MonoBehaviour, ISaveLoadObject
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
    private bool isInitialized;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        mapTilemap = GetComponent<Tilemap>();
        mapTilemapInfo = GetComponent<TilemapInfo>();
        timerBetweenMapUpdates = new WaitForSeconds(delayBetweenMapUpdates);
        if (!isInitialized)
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
        isInitialized = true;
        foreach (var tile in groundTilemapInfo.allTiles)
        {
            mapTilemap.SetTile(tile.Key, exploredTile);
            mapTilemap.SetColor(tile.Key, Color.black);
        }
        mapTilemapInfo.UpdateAllTiles();
        isInitialized = true;
    }
    
    private IEnumerator MapExplorerCoroutine()
    {
        while (true)
        {
            switch (G.characters.currentCharacter)
            {
                case var obj when obj == G.characters.lightSide:
                    ExploreMap(whenInLightSideColliders);
                    break;
                case var obj when obj == G.characters.darkSide:
                    ExploreMap(whenInDarkSideColliders);
                    break;
                case var obj when obj == G.characters.equilibrium:
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
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        List<Vector3Int> serializedExploredTilesPositions = new ();
        foreach (Vector3Int position in mapTilemapInfo.allTiles.Keys)
        {
            if(mapTilemap.GetColor(position) == Color.white)
                serializedExploredTilesPositions.Add(position);
        }
        return new ObjectSaveLoadData(objectId, new System.Object[] { serializedExploredTilesPositions });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        BuildUnexploredMap();
        //data[0] - exploredTilesPositions
        List<Vector3Int> serializedExploredTilesPositions = ((JArray)dataToUnpack.data[0]).ToObject<List<Vector3Int>>();
        foreach (Vector3Int position in serializedExploredTilesPositions)
        {
            mapTilemap.SetColor(position, Color.white);
        }
    }
}
