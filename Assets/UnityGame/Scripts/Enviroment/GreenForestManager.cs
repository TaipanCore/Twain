using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GreenForestManager : MonoBehaviour
{
    [SerializeField, Min(0)] private float delayBetweenFirefliesSpawn;
    private WaitForSeconds firefliesSpawnTimer;
    [SerializeField, Min(0)] private float delayBetweenTravelerFirefliesSpawn;
    private WaitForSeconds travelerFireflySpawnTimer;
    [SerializeField, Min(0)] private float fireflyLifetime;
    [SerializeField] private Transform[] fireflySpawnPoints;
    [SerializeField] private Transform travelerFireflySpawnPoint;
    
    private GameObjectsPool blueFireflyPool;
    private GameObjectsPool greenFireflyPool;
    private bool playerInForest;
    
    void Start()
    {
        blueFireflyPool = GameObject.Find("BlueFireflyPool").GetComponent<GameObjectsPool>();
        greenFireflyPool = GameObject.Find("GreenFireflyPool").GetComponent<GameObjectsPool>();
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
    private void FireflyLifeCycle(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        GameObjectsPool pool = Random.Range(0, 2) == 0 ? blueFireflyPool : greenFireflyPool;
        GameObject firefly = pool.Get(_ => { });
        firefly.transform.position = startPosition;
        firefly.GetComponent<FireflyMovement>().MoveAlongPath(endPosition, duration).OnComplete(() => pool.Return(firefly, _ => { }));
    }
}

