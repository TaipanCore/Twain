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
        set
        {
            _damage = value;
        }
    }
    private bool _isAggro;
    public bool isAggro
    {
        get => _isAggro;
        set
        {
            _isAggro = value;
        }
    }

    [SerializeField] private float cancelHuntPathLenght;
    [SerializeField] private float timeToCancelHunt;
    private WaitForSeconds cancelHuntTimer;
    [SerializeField] private GameObject smokeParticlesPrefab;

    private ClotMovement movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Coroutine currentCancelHuntCoroutine;
    private State state;

    private Coroutine retreatCoroutine;

    private void Start()
    {
        movement = GetComponent<ClotMovement>();
        animator =  GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void SetState(State newState)
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
    private void SetIdleSettings()
    {
        isAggro = false;
        movement.target = transform;
    }
    private void SetHuntSettings()
    {
        movement.target = GameManager.lightSide.GetComponent<Transform>();
    }
    private void SetRetreatSettings()
    {
        movement.target = GameManager.equilibrium.GetComponent<Transform>();
        if (retreatCoroutine == null)
            retreatCoroutine = StartCoroutine(RetreatCoroutine());
    }
    private void Idle()
    {
        if (isAggro)
        {
            SetState(State.Hunt);
        }
    }
    private void Hunt()
    {
        movement.navMeshAgent.SetDestination(movement.target.position);
        if (GameManager.isUnited)
        {
            SetState(State.Retreat);
        }
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
    }
    private IEnumerator RetreatCoroutine()
    {
        while (true)
        {
            if (!movement.isInPanic || (movement.isInPanic && !Utils.IsAgentMoving(movement.navMeshAgent)))
            {
                movement.navMeshAgent.SetDestination(movement.GetRetreatPosition());
            }
            yield return null;
        }
    }
    public void ApplyStun(float time)
    {
        StartCoroutine(StunCoroutine(time));
    }
    public void ReceiveDamage(float damage)
    {
        if (invulnerableTimer <= 0f)
        {
            hitpoints -= damage;
            GiveInvulnerability();
        }
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
    public void DealDamage(float damage, IDamageReceiver target)
    {
        target.ReceiveDamage(damage);
    }
    /*
    private void OnDrawGizmos()
    {      
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target, retreatDistance);
    }
    */
}
