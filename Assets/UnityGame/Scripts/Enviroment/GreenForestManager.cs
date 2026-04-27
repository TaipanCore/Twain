using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GreenForestManager : MonoBehaviour
{
    [SerializeField, Min(0)] private float delayBetweenFirefliesSpawn;
    [SerializeField, Min(0)] private float fireflyLifetime;
    [SerializeField] private Transform[] fireflySpawnPoints;
    
    private GameObjectsPool fireflyPool;
    private bool playerInForest;
    private Coroutine fireflyCoroutine;
    
    void Start()
    {
        fireflyPool = GameObject.Find("FireflyPool").GetComponent<GameObjectsPool>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarkSideBehaviour _))
        {
            playerInForest = true;
            fireflyCoroutine ??= StartCoroutine(SpawnFireflies());
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
            GameObject firefly = fireflyPool.Get((GameObject _) => { });
            Transform[] startAndEndPoints = Utils.GetRandomElements(fireflySpawnPoints, 2);
            firefly.transform.position = startAndEndPoints[0].position;
            firefly.GetComponent<FireflyMovement>().MoveAlongPath(startAndEndPoints[1].position, fireflyLifetime).OnComplete(() => fireflyPool.Return(firefly, (GameObject _) => { }));
            yield return new WaitForSeconds(delayBetweenFirefliesSpawn);
        }
        fireflyCoroutine = null;
    }
}

