using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityGame.Scripts.Interfaces;

public class ClotBehaviour : MonoBehaviour, IDamageDealer, IInvulnerableDamageReceiver, IAbleAggro, IStunnable
{
    private static readonly int PlayShock = Animator.StringToHash("PlayShock");
    private static readonly int IdleAnim = Animator.StringToHash("Idle");

    public enum State
    {
        Idle,
        Hunt,
        Retreat
    }

    [SerializeField] private float _hitpoints;
    public float hitpoints
    {
        get => _hitpoints;
        set
        {
            _hitpoints = value;
            if (_hitpoints <= 0)
                Die();
        }
    }
    [SerializeField] private float _invulnerableTime;
    private float invulnerableTimer;
    public float invulnerableTime
    {
        get => _invulnerableTime;
        set => _invulnerableTime = value;
    }
    
    [SerializeField] private float _damage;
    public float damage
    {
        get => _damage;
        set => _damage = value;
    }

    private bool _isAggro;
    public bool isAggro
    {
        get => _isAggro;
        set
        {
            if (!(isIgnoreLight && value))
            {
                _isAggro = value;
            }
        }
    }
    
    public bool isIgnoreLight { get; private set; }

    [SerializeField] private float cancelHuntPathLenght;
    [SerializeField] protected float timeToCancelHunt;
    protected WaitForSeconds cancelHuntTimer;
    [SerializeField] private GameObject smokeParticlesPrefab;

    protected ClotMovement movement;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D objectCollider;
    private Coroutine currentCancelHuntCoroutine;
    private Coroutine stunCoroutine;
    private State state;

    private Coroutine retreatCoroutine;

    private void Start()
    {
        movement = GetComponent<ClotMovement>();
        animator =  GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
        cancelHuntTimer = new WaitForSeconds(timeToCancelHunt);
        SetState(State.Idle);
    }
    private void Update()
    {
        if (invulnerableTimer > 0f)
            invulnerableTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Idle:
                Idle();
                break;                
            case State.Hunt:
                Hunt();
                break;
            case State.Retreat:
                Retreat();
                break;
        }
    }

    protected void SetState(State newState)
    {
        state = newState;
        switch (state)
        {
            case State.Idle:
                SetIdleSettings();
                break;
            case State.Hunt:
                SetHuntSettings();
                break;
            case State.Retreat:
                SetRetreatSettings();
                break;
        }
    }
    public State GetState()
    {
        return state;
    }
    protected virtual void SetIdleSettings()
    {
        isAggro = false;
        movement.SetMoveState(State.Idle);
    }
    private void SetHuntSettings()
    {
        movement.SetMoveState(State.Hunt);
    }
    private void SetRetreatSettings()
    {
        movement.SetMoveState(State.Retreat);
        retreatCoroutine ??= StartCoroutine(RetreatCoroutine());
    }
    protected virtual void Idle()
    {
        if (isAggro)
        {
            SetState(State.Hunt);
        }
    }
    private void Hunt()
    {
        if (!isAggro)
        {
            SetState(State.Idle);
            return;
        }
        if (GameManager.isUnited)
        {
            StopCoroutine(currentCancelHuntCoroutine);
            currentCancelHuntCoroutine = null;
            SetState(State.Retreat);
            return;
        }
        movement.navMeshAgent.SetDestination(movement.target.position);
        if (Utils.GetPathLength(movement.navMeshAgent.path) >= cancelHuntPathLenght)
        {
            currentCancelHuntCoroutine ??= StartCoroutine(CancelHuntCoroutine());
        }
        else
        {
            if (currentCancelHuntCoroutine != null)
            {
                StopCoroutine(currentCancelHuntCoroutine);
                currentCancelHuntCoroutine = null;
            }
        }
    }
    private void Retreat()
    {
        if (!isAggro)
        {
            SetState(State.Idle);
            return;
        }
        if (!GameManager.isUnited)
        {
            StopCoroutine(retreatCoroutine);
            retreatCoroutine = null;
            SetState(State.Hunt);
        }
    }

    private IEnumerator CancelHuntCoroutine()
    {
        yield return cancelHuntTimer;
        SetState(State.Idle);
    }
    private IEnumerator StunCoroutine(float stunTime)
    {
        movement.navMeshAgent.isStopped = true;
        animator.SetTrigger(PlayShock);
        yield return new WaitForSeconds(stunTime);
        animator.Play(IdleAnim);
        movement.navMeshAgent.isStopped = false;
        stunCoroutine = null;
    }
    private IEnumerator RetreatCoroutine()
    {
        while (true)
        {
            if (!movement.isInPanic || (movement.isInPanic && !Utils.IsAgentMoving(movement.navMeshAgent)))
            {
                movement.navMeshAgent.SetPath(movement.GetOnCirclePositionForCurrentState());
            }
            yield return null;
        }
    }
    public void ApplyStun(float time)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = StartCoroutine(StunCoroutine(time));
        }
    }
    public void ReceiveDamage(float receivedDamage)
    {
        if (invulnerableTimer <= 0f)
        {
            hitpoints -= receivedDamage;
            GiveInvulnerability();
        }
    }
    public IEnumerator IgnoreLight(float time)
    {
        isIgnoreLight = true;
        yield return new WaitForSeconds(time);
        isIgnoreLight = false;
        objectCollider.enabled = false;
        objectCollider.enabled = true;
    }
    public void GiveInvulnerability()
    {
        Tween blinkingTween = spriteRenderer.DOFade(0.4f, 0.15f).SetLoops(-1, LoopType.Yoyo);
        DOVirtual.DelayedCall(invulnerableTime, () =>
        {
            blinkingTween.Kill();
            spriteRenderer.DOFade(1f, 0f);
        });
        invulnerableTimer = invulnerableTime;
    }
    public void Die()
    {
        ParticleSystem smokeParticles = Instantiate(smokeParticlesPrefab, transform.position + new Vector3(0.15f, 0.5f), Quaternion.identity).GetComponent<ParticleSystem>();
        spriteRenderer.DOFade(0f, smokeParticles.main.startLifetime.constantMin).SetEase(Ease.InQuad).OnComplete(() => Destroy(gameObject));
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageReceiver damageReceiver) && other.gameObject.IsInLayerMask(GameManager.playerMask))
        {
            DealDamage(damage, damageReceiver);
        }
    }
    public void DealDamage(float dealedDamage, IDamageReceiver target)
    {
        target.ReceiveDamage(dealedDamage);
    }
}
