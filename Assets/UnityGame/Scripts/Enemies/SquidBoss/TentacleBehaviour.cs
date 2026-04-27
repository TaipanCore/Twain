using System;
using DG.Tweening;
using UnityEngine;

public class TentacleBehaviour : MonoBehaviour
{
    [SerializeField] private Transform firefly;
    
    [HideInInspector] public event Action OnDefeated;
    [HideInInspector] public float timeToReachTarget;
    [HideInInspector] public float timeToRetreat;
    
    private SimpleAnimatorWithColliders tentacleAnimator;
    private Vector3 startPosition;
    private Tween attackTween;
    private Tween retreatTween;
    private ParticleSystem darkEmitParticles;
    private Transform darkEmitParticlesTransform;
    
    private void Start()
    {
        startPosition = transform.position;
        tentacleAnimator = GetComponent<SimpleAnimatorWithColliders>();
        darkEmitParticlesTransform = transform.Find("DarkEmitParticles");
        darkEmitParticles = darkEmitParticlesTransform.GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LightSideBehaviour _) && !retreatTween.IsActive())
        {
            darkEmitParticlesTransform.position = collision.transform.position;
            darkEmitParticles.Play();
            Retreat();
        }
    }
    
    public void Attack()
    {
        if (!retreatTween.IsActive() && !attackTween.IsActive())
        {
            attackTween = transform.DOMove(firefly.position, timeToReachTarget)
                .OnComplete(() => firefly.Find("CircleLight").GetComponent<CircleLight>().SetRange(0f, 0.25f));
        }
    }
    public void Retreat()
    {
        if (!retreatTween.IsActive())
        {
            attackTween?.Kill();
            int baseFramerate = tentacleAnimator.GetFramerate();
            retreatTween = transform.DOMove(startPosition, timeToRetreat)
                .OnStart(() => tentacleAnimator.SetFramerate(baseFramerate * 2))
                .OnComplete(() => tentacleAnimator.SetFramerate(baseFramerate));
            OnDefeated?.Invoke();
        }
    }
}
