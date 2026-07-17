using System;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityGame.Scripts.Interfaces;

public class ClotBehaviour : MonoBehaviour, IDamageDealer, IInvulnerableDamageReceiver, IAbleAggro, IStunnable, ISaveLoadObject
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
    [SerializeField] private GameObject smokeParticlesPrefab;
    [SerializeField] private EtherSpawner etherSpawner;

    protected ClotMovement movement;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D objectCollider;
    private Tween cancelHuntTween;
    private Tween stunTween;
    private State state;
    
    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        movement = GetComponent<ClotMovement>();
        animator =  GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectCollider = GetComponent<Collider2D>();
        G.characters.PlayerDied += OnPlayerDied;
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
        isAggro = true;
        movement.SetMoveState(State.Hunt);
    }
    private void SetRetreatSettings()
    {
        isAggro = true;
        movement.SetMoveState(State.Retreat);
        StartCoroutine(RetreatCoroutine());
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
        if (G.characters.isUnited)
        {
            if (cancelHuntTween != null)
            {
                cancelHuntTween.Kill();
                cancelHuntTween = null;
            }
            SetState(State.Retreat);
            return;
        }
        movement.navMeshAgent.SetDestination(movement.target.position);
        if (Utils.GetPathLength(movement.navMeshAgent.path) >= cancelHuntPathLenght)
        {
            cancelHuntTween ??= StartCancelHuntTween(timeToCancelHunt);
        }
        else
        {
            if (cancelHuntTween != null)
            {
                cancelHuntTween.Kill();
                cancelHuntTween = null;
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
        if (!G.characters.isUnited)
        {
            SetState(State.Hunt);
        }
    }

    private Tween StartCancelHuntTween(float time)
    {
        return DOVirtual.DelayedCall(time, () => SetState(State.Idle), false);
    }
    private Tween StartStunTween(float time)
    {
        movement.navMeshAgent.isStopped = true;
        animator.SetTrigger(PlayShock);
        return DOVirtual.DelayedCall(time, () =>
        {
            animator.Play(IdleAnim);
            movement.navMeshAgent.isStopped = false;
            stunTween = null;
        }, false);
    }
    private IEnumerator RetreatCoroutine()
    {
        while (state == State.Retreat)
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
        stunTween?.Kill();
        stunTween = StartStunTween(time);
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
        },false);
        invulnerableTimer = invulnerableTime;
    }
    public void Die()
    {
        G.enemiesDieStates.SetDieState(objectId);
        G.characters.PlayerDied -= OnPlayerDied;
        ParticleSystem smokeParticles = Instantiate(smokeParticlesPrefab, transform.position + new Vector3(0.15f, 0.5f), Quaternion.identity).GetComponent<ParticleSystem>();
        spriteRenderer.DOFade(0f, smokeParticles.main.startLifetime.constantMin).SetEase(Ease.InQuad).OnComplete(() => Destroy(gameObject));
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageReceiver damageReceiver) && other.gameObject.IsInLayerMask(G.playerMask))
        {
            DealDamage(damage, damageReceiver);
        }
    }

    private void OnPlayerDied()
    {
        isAggro = false;
    }
    public void DealDamage(float dealedDamage, IDamageReceiver target)
    {
        target.ReceiveDamage(dealedDamage);
    }

    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        float cancelHuntTweenRemainingSeconds = cancelHuntTween.IsActive() ? timeToCancelHunt - cancelHuntTween.Elapsed(false) : 0f;
        float stunTweenRemainingSeconds = stunTween.IsActive() ? stunTween.Duration() - stunTween.Elapsed(false) : 0f;
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            hitpoints,
            state,
            transform.position,
            invulnerableTimer,
            cancelHuntTweenRemainingSeconds,
            stunTweenRemainingSeconds,
            etherSpawner.etherCount
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - hitpoints
        if (float.TryParse(dataToUnpack.data[0].ToString(), out var parsedHitpoints))
            hitpoints = parsedHitpoints;
        //data[1] - state
        if (Enum.TryParse(dataToUnpack.data[1].ToString(), out State parsedState))
            SetState(parsedState);
        //data[2] - position
        movement.navMeshAgent.Warp(((JObject)dataToUnpack.data[2]).ToObject<Vector3>());
        //data[3] - invulnerableTimer
        if (float.TryParse(dataToUnpack.data[3].ToString(), out var parsedInvulnerableTimer))
            invulnerableTimer = parsedInvulnerableTimer;
        //data[4] - cancelHuntTweenRemainingSeconds
        if (float.TryParse(dataToUnpack.data[4].ToString(), out var parsedCancelHuntTweenRemainingSeconds))
            if (!Mathf.Approximately(parsedCancelHuntTweenRemainingSeconds, 0f))
                cancelHuntTween = StartCancelHuntTween(parsedCancelHuntTweenRemainingSeconds);
        //data[5] - stunTweenRemainingSeconds
        if (float.TryParse(dataToUnpack.data[5].ToString(), out var parsedStunTweenRemainingSeconds))
            if (!Mathf.Approximately(parsedStunTweenRemainingSeconds, 0f))
                stunTween = StartStunTween(parsedStunTweenRemainingSeconds);
        //data[6] - etherCount
        if (int.TryParse(dataToUnpack.data[6].ToString(), out var parsedEtherCount))
            etherSpawner.etherCount = parsedEtherCount;
    }
}
