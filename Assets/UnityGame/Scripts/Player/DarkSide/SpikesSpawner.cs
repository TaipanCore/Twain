using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private float spikesLifetime;
    
    private Transform objectTransform;
    private Vector3 direction;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Tilemap tilemap;
    private SpikesSounds spikesSounds;
    private bool canBeDestroyed;
    
    private Dictionary<Vector3, Tween> activeSpikes;
    
    private void Start()
    {
        objectTransform = GetComponent<Transform>();
        spikesSpawnDelay = new WaitForSeconds(spikesSpawnTimeInterval);
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        spikesSounds = GetComponent<SpikesSounds>();
    }

    private void Update()
    {
        if (objectTransform.childCount == 0 && canBeDestroyed)
        {
            Destroy(gameObject);
        }
    }
    
    public IEnumerator SpawnSpikes()
    {
        yield return null;
        direction = (G.mouseTracker.mousePosition - transform.position).normalized;
        startPosition = transform.position + direction * spawnPositionOffset;
        endPosition = startPosition + (numberOfSpikes - 1) * stepBetweenSpikes * direction;
        SpawnParticles();
        for (int i = 0; i < numberOfSpikes; i++)
        {
            Vector3 position = startPosition + i * stepBetweenSpikes * direction;
            if (tilemap.HasTile(tilemap.WorldToCell(position)))
            {
                CreateSpike(position, spikesLifetime);
            }
            yield return spikesSpawnDelay;
        }
        canBeDestroyed = true;
    }

    private void CreateSpike(Vector3 position, float remainingLifetime)
    {
        GameObject spike = Instantiate(spikePrefab, position, Quaternion.identity, transform);
        spikesSounds.PlaySpikeSpawnSound(position);
        spike.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-position.y * 100);
        Tween destroyTween = DOVirtual.DelayedCall(remainingLifetime, () => Destroy(spike), false)
            .OnComplete(() =>
            {
                spikesSounds.PlaySpikeDespawnSound(position);
                activeSpikes.Remove(spike.transform.position);
            });
        activeSpikes.Add(spike.transform.position, destroyTween);
    }

    public void RestoreSpawnedSpikes(SpikeData[] spikesData)
    {
        foreach (SpikeData spikeData in spikesData)
            CreateSpike(spikeData.position, spikeData.remainingLifetime);
        canBeDestroyed = true;
    }
    
    public void Initialize(int numberOfSpikes, float spikesLifetime, Dictionary<Vector3, Tween> activeSpikes)
    {
        this.numberOfSpikes = numberOfSpikes;
        this.spikesLifetime = spikesLifetime;
        this.activeSpikes = activeSpikes;
    }

    private void SpawnParticles()
    {
        ParticleSystem particles = Instantiate(particlesPrefab, (endPosition + startPosition) / 2f, Quaternion.FromToRotation(Vector3.up, endPosition - startPosition), objectTransform);
        ParticleSystem.MainModule main = particles.main;
        main.duration = spikesLifetime;
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.scale = new Vector3(shape.scale.x, numberOfSpikes * stepBetweenSpikes, shape.scale.z);
        particles.Play();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);
    }

    [Serializable]
    public class SpikeData
    {
        public SpikeData(Vector3 position, float remainingLifetime)
        {
            this.position = position;
            this.remainingLifetime = remainingLifetime;
        }
        
        public Vector3 position;
        public float remainingLifetime;
    }
}
