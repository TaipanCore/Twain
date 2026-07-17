using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using FireflyData = FireflyMovement.FireflyData;

public class GreenForestManager : MonoBehaviour, ISaveLoadObject
{
    [SerializeField, Min(0)] private float delayBetweenFirefliesSpawn;
    private WaitForSeconds firefliesSpawnTimer;
    [SerializeField, Min(0)] private float delayBetweenTravelerFirefliesSpawn;
    private WaitForSeconds travelerFireflySpawnTimer;
    [SerializeField, Min(0)] private float fireflyLifetime;
    [SerializeField] private Transform[] fireflySpawnPoints;
    [SerializeField] private Transform travelerFireflySpawnPoint;
    [SerializeField] private GameObjectsPool blueFireflyPool;
    [SerializeField] private GameObjectsPool greenFireflyPool;

    private Dictionary<GameObject, GameObjectsPool> activeFirefliesAndPools = new ();
    private bool playerInForest;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        firefliesSpawnTimer = new WaitForSeconds(delayBetweenFirefliesSpawn);
        travelerFireflySpawnTimer = new WaitForSeconds(delayBetweenTravelerFirefliesSpawn);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarkSideBehaviour _))
        {
            playerInForest = true;
            StartCoroutine(SpawnFireflies());
            StartCoroutine(SpawnTravelerFirefly());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarkSideBehaviour _))
        {
            playerInForest = false;
        }
    }

    private IEnumerator SpawnFireflies()
    {
        while (playerInForest)
        {
            Transform[] startAndEndPoints = Utils.GetRandomElements(fireflySpawnPoints, 2);
            FireflyLifeCycle(startAndEndPoints[0].position, startAndEndPoints[1].position, fireflyLifetime);
            yield return firefliesSpawnTimer;
        }
    }
    private IEnumerator SpawnTravelerFirefly()
    {
        while (playerInForest)
        {
            FireflyLifeCycle(travelerFireflySpawnPoint.position, fireflySpawnPoints[0].position, fireflyLifetime);
            yield return travelerFireflySpawnTimer;
        }
    }
    private void FireflyLifeCycle(Vector3 startPosition, Vector3 endPosition, float duration, Vector3[] path = null, GameObjectsPool pool = null)
    {
        pool ??= Random.Range(0, 2) == 0 ? blueFireflyPool : greenFireflyPool;
        GameObject firefly = pool.Get();
        activeFirefliesAndPools.Add(firefly, pool);
        firefly.transform.position = startPosition;
        firefly.GetComponent<FireflyMovement>().MoveAlongPath(endPosition, duration, path).OnComplete(() =>
        {
            activeFirefliesAndPools.Remove(firefly);
            pool.Return(firefly);
        });
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        List<FireflyData> firefliesData = new ();
        foreach (KeyValuePair<GameObject, GameObjectsPool> pair in activeFirefliesAndPools)
        {
            char color = pair.Value == blueFireflyPool ? 'b' : 'g';
            firefliesData.Add(pair.Key.GetComponent<FireflyMovement>().PackFireflyData(color));
        }
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            firefliesData
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - firefliesData
        FireflyData[] firefliesData = ((JArray)dataToUnpack.data[0]).ToObject<FireflyData[]>();
        foreach (FireflyData data in firefliesData)
        {
            GameObjectsPool pool = data.color == 'b' ? blueFireflyPool : greenFireflyPool;
            FireflyLifeCycle(data.position, data.restOfPath.Last(), data.pathRestOfTime, data.restOfPath, pool);
        }
    }
}

