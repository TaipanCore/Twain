using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] float stepBetweenSpikes;
    [SerializeField, Min(0)] private float spawnPositionOffset;
    [SerializeField, Min(0)] float spikesSpawnTimeInterval;
    private WaitForSeconds spikesSpawnDelay;
    
    private int numberOfSpikes;
    float spikesLifetime;
    
    private Transform objectTransform;
    private Vector3 direction;
    private Vector3 startPosition;
    private Tilemap tilemap;
    
    void Start()
    {
        objectTransform = GetComponent<Transform>();
        spikesSpawnDelay = new WaitForSeconds(spikesSpawnTimeInterval);
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        StartCoroutine(SpawnSpikes());
    }

    void LateUpdate()
    {
        if (objectTransform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator SpawnSpikes()
    {
        direction = (MouseTracker.mousePosition - transform.position).normalized;
        startPosition = transform.position + direction * spawnPositionOffset;
        for (int i = 0; i < numberOfSpikes; i++)
        {
            Vector3 position = startPosition + i * stepBetweenSpikes * direction;
            if (tilemap.HasTile(tilemap.WorldToCell(position)))
            {
                GameObject spike = Instantiate(spikePrefab, position, Quaternion.identity, transform);
                spike.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-position.y * 100);
                Destroy(spike, spikesLifetime);
            }
            yield return spikesSpawnDelay;
        }
    }

    public void Initialize(int numberOfSpikes, float spikesLifetime)
    {
        this.numberOfSpikes = numberOfSpikes;
        this.spikesLifetime = spikesLifetime;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, startPosition + (numberOfSpikes - 1) * stepBetweenSpikes * direction);
    }
}
