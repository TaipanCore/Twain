using System;
using DG.Tweening;
using UnityEngine;

public class TentacleBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attack,
        Retreat
    }
    
    [SerializeField] private Transform firefly;
    [SerializeField] private SimpleAnimatorWithColliders tentacleAnimator;
    [SerializeField] private Transform startTransform;

    [HideInInspector] public State currentState;
    public event Action OnDefeated;
    
    [HideInInspector] public float timeToReachTarget;
    [HideInInspector] public float timeToRetreat;
    
    private Tween attackTween;
    private Tween retreatTween;
    
    
    private ParticleSystem darkEmitParticles;
    private Transform darkEmitParticlesTransform;
    
    private void Start()
    {
        darkEmitParticlesTransform = transform.Find("DarkEmitParticles");
        darkEmitParticles = darkEmitParticlesTransform.GetComponent<ParticleSystem>();
        currentState = State.Idle;
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
            currentState = State.Attack;
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
            currentState = State.Retreat;
            retreatTween = transform.DOMove(startTransform.position, timeToRetreat)
                .OnStart(() => tentacleAnimator.SetFramerate(baseFramerate * 2))
                .OnComplete(() =>
                {
                    currentState = State.Idle;
                    tentacleAnimator.SetFramerate(baseFramerate);
                });
            OnDefeated?.Invoke();
        }
    }

    public float GetStateElapsedTime()
    {
        switch (currentState)
        {
            case State.Attack:
                return attackTween.Elapsed(false);
            case State.Retreat:
                return retreatTween.Elapsed(false);
            default:
                return 0f;
        }
    }
    
    [Serializable]
    public class TentacleData
    {
        public TentacleData(Vector3 position, State state, float stateElapsedTime)
        {
            this.position = position;
            this.state = state;
            this.stateElapsedTime = stateElapsedTime;
        }
        
        public Vector3 position;
        public State state;
        public float stateElapsedTime;
    }
}
