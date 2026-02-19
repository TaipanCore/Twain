using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SpikesSpawner : MonoBehaviour
{
    [Header("Spikes")]
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] float stepBetweenSpikes;
    [SerializeField, Min(0)] private float spawnPositionOffset;
    [SerializeField, Min(0)] float spikesSpawnTimeInterval;
    private WaitForSeconds spikesSpawnDelay;

    [Header("Particles")]
    [SerializeField] private ParticleSystem particlesPrefab;
    
    private int numberOfSpikes;
    float spikesLifetime;
    
    private Transform objectTransform;
    private Vector3 direction;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Tilemap tilemap;
    
    void Start()
    {
        objectTransform = GetComponent<Transform>();
        spikesSpawnDelay = new WaitForSeconds(spikesSpawnTimeInterval);
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        StartCoroutine(SpawnSpikes());
    }

    void Update()
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
        endPosition = startPosition + (numberOfSpikes - 1) * stepBetweenSpikes * direction;
        SpawnParticles();
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

    private void SpawnParticles()
    {
        ParticleSystem particles = Instantiate(particlesPrefab, (endPosition + startPosition) / 2f, Quaternion.FromToRotation(Vector3.up, endPosition - startPosition), objectTransform);
        ParticleSystem.MainModule main = particles.main;
        main.duration =  spikesLifetime;
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.scale = new Vector3(shape.scale.x, numberOfSpikes * stepBetweenSpikes, shape.scale.z);
        particles.Play();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}
