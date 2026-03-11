using DG.Tweening;
using UnityEngine;
public class EtherSpawner : MonoBehaviour, IEtherContainer
{
    [SerializeField] private AnimationCurve etherSpawnRate;
    [SerializeField] private float spreadRange;
    [SerializeField] private int _etherCount;
    public int etherCount
    {
        get => _etherCount;
        set
        {
            _etherCount = value;
        }
    }
    private int maxEtherCount;

    private ShrineOfBalanceBehaviour shrineBehaviour;
    private Transform target;
    private GameObjectsPool etherPool;
    private float etherSpawnCooldownTimer;
    private Transform etherSpawnPoint;

    private void Start()
    {
        shrineBehaviour = GameManager.shrineOfBalance.GetComponent<ShrineOfBalanceBehaviour>();
        target = shrineBehaviour.transform.Find("EtherMagnetPoint").GetComponent<Transform>();
        etherPool = GameObject.Find("EtherPool").GetComponent<GameObjectsPool>();
        etherSpawnPoint = transform.Find("EtherSpawnPoint");
        maxEtherCount = etherCount;
    }
    private void Update()
    {
        if (etherSpawnCooldownTimer > 0f)
            etherSpawnCooldownTimer -= Time.deltaTime;
    }
    public void SpawnEtherParticle()
    {
        if (etherSpawnCooldownTimer <= 0f && etherCount > 0)
        {
            GameObject etherParticle = etherPool.Get(GetAction);
            Sequence followPath = DOTween.Sequence();
            followPath
                .Append(etherParticle.transform.DOMove((Vector2)etherSpawnPoint.position + Random.insideUnitCircle * spreadRange, 0.75f).SetEase(Ease.OutCubic))
                .Append(etherParticle.transform.DOPath(GeneratePath(), 1.5f, PathType.CubicBezier, resolution: 5).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    shrineBehaviour.etherCount++;
                    etherPool.Return(etherParticle, ReturnAction);
                }));
            etherCount--;
            etherSpawnCooldownTimer = etherSpawnRate.Evaluate((float)etherCount / maxEtherCount);
        }
    }

    private Vector3[] GeneratePath()
    {
        return new Vector3[]
        {
            target.position,
            (target.position + etherSpawnPoint.position) * 0.5f + (Vector3)Random.insideUnitCircle * spreadRange,
            target.position + (Vector3)Random.insideUnitCircle * 2f * spreadRange
        };
    }
    private void GetAction(GameObject gameObj)
    {
        gameObj.transform.position = etherSpawnPoint.position;
    }
    private void ReturnAction(GameObject gameObj)
    {
        gameObj.GetComponent<TrailRenderer>().Clear();
    }
}
