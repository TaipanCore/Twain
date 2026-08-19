using System;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Owner = ShardBehaviour.Owner;

public class EvilSpiritBehaviour : MonoBehaviour, IDamageReceiver, IReactToFocusedLight, IAbleAggro, ISaveLoadObject
{
    private static readonly int PlayIdle = Animator.StringToHash("PlayIdle");
    private static readonly int PlayHunt = Animator.StringToHash("PlayHunt");
    private static readonly int PlayAttack = Animator.StringToHash("PlayAttack");
    private static readonly int PlayDash = Animator.StringToHash("PlayDash");
    private static readonly int PlayDie = Animator.StringToHash("PlayDie");
    
    
    public enum State
    {
        Idle,
        Hunt,
        Attack,
        Dash,
        Die
    }
    
    [SerializeField] private GameObject redKeyShard;
    [Header("Navigation")]
    [SerializeField] private Transform target;
    [SerializeField] private float maxDistanceToTarget;
    [SerializeField] private float maxDistanceToTargetAfterDash;
    [SerializeField] private float moveSpeed;
    [Header("Dash")]
    [SerializeField] private float timeInLightToDash;
    [SerializeField] private AnimationCurve dashDuration;
    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;

    private bool _isInFocusedLight;

    public bool isInFocusedLight
    {
        get =>  _isInFocusedLight;
        set
        {
            _isInFocusedLight = value;
            if (_isInFocusedLight)
            {
                inLightSounds ??= evilSpiritSounds.PlayInFocusedLightSound();
                steamParticles.Play();
            }
            else
            {
                inLightSounds?.Stop();
                inLightSounds = null;
                steamParticles.Stop();
            }
        }
    }

    private bool _isAggro;
    public bool isAggro
    {
        get => _isAggro;
        set
        {
            if (!isAggro && value)
            {
                evilSpiritSounds.PlayAggroSound();
                G.audio.PlayMusic(G.music.evilSpiritMusic, fadeDuration: 5f);
            }
            _isAggro = value;
        }
        
    }
    
    [Header("Health")]
    [SerializeField] private float _hitpoints;
    public float hitpoints
    {
        get => _hitpoints;
        set
        {
            _hitpoints = value;
            if (_hitpoints <= 0 && GetState() != State.Die)
            {
                SetState(State.Die);
            }
        }
    }

    private float dashTimer;
    private TrailRenderer redEyeTrail;
    private ParticleSystem steamParticles;
    private GameObject redKeyShardImage;
    private float maxHitpoints;
    private Vector2 movement;
    private Vector3 previousPosition;
    private Animator animator;
    private EvilSpiritSounds evilSpiritSounds;
    private AudioSource inLightSounds;
    private State state;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
        redEyeTrail = transform.Find("RedEye").GetComponent<TrailRenderer>();
        steamParticles = transform.Find("SteamParticles").GetComponent<ParticleSystem>();
        transform.Find("RedEye").GetComponent<RedEyeBehaviour>().damage = damage;
        redKeyShardImage = transform.Find("RedKeyShardImage").gameObject;
        animator = GetComponent<Animator>();
        evilSpiritSounds = GetComponent<EvilSpiritSounds>();
        maxHitpoints = hitpoints;
        dashTimer = timeInLightToDash;
    }
    private void Start()
    {
        ShardBehaviour redShardBehaviour = redKeyShard.GetComponent<ShardBehaviour>();
        if (redShardBehaviour.owner == Owner.None)
            redShardBehaviour.owner = Owner.Enemy;
        G.characters.PlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        G.characters.PlayerDied -= OnPlayerDied;
    }
    private void FixedUpdate()
    {
        if (isInFocusedLight)
        {
            dashTimer -= Time.fixedDeltaTime;
            hitpoints -= Time.fixedDeltaTime;
        }
        switch (state)
        {
            case State.Idle:
                Idle();
                break;                
            case State.Hunt:
                Hunt();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Dash:
                Dash();
                break;
        }
    }
    
    private void SetState(State newState)
    {
        animator.ResetTrigger("Play" + state);
        state = newState;
        switch (state)
        {
            case State.Idle:
                SetIdleSettings();
                break;
            case State.Hunt:
                SetHuntSettings();
                break;
            case State.Attack:
                SetAttackSettings();
                break;
            case State.Dash:
                SetDashSettings();
                break;
            case State.Die:
                SetDieSettings();
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
        G.audio.PlayMusic(G.music.labyrinthMusic);
        redKeyShardImage.SetActive(true);
        redEyeTrail.emitting = false;
        animator.SetTrigger(PlayIdle);
    }
    private void SetHuntSettings()
    {
        redKeyShardImage.SetActive(true);
        redEyeTrail.emitting = false;
        animator.SetTrigger(PlayHunt);
    }
    private void SetAttackSettings()
    {
        redKeyShardImage.SetActive(false);
        redEyeTrail.emitting = true;
        animator.SetTrigger(PlayAttack);
    }
    private void SetDashSettings()
    {
        redKeyShardImage.SetActive(false);
        redEyeTrail.emitting = true;
        int countOfDashes = hitpoints / maxHitpoints > 0.5f ? Random.Range(1, 4) : Random.Range(3, 5);
        StartCoroutine(DoDashes(countOfDashes));
        animator.SetTrigger(PlayDash);
    }

    private void SetDieSettings()
    {
        redEyeTrail.emitting = false;
        G.audio.PlayMusic(G.music.labyrinthMusic);
        Die();
        animator.SetTrigger(PlayDie);
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
        if (dashTimer <= 0 || !Utils.IsInRange(transform.position, target.position, maxDistanceToTarget))
        {
            dashTimer = timeInLightToDash;
            SetState(State.Dash);
        }
        if (Utils.IsInRange(transform.position, target.position, attackRange))
        {
            SetState(State.Attack);
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.fixedDeltaTime);
        movement = transform.position - previousPosition;
        previousPosition = transform.position;
        FlipSprite();
    }

    private void Attack() { }
    private void Dash() { }
    
    private IEnumerator DoDashes(int countOfDashes)
    {
        for (int i = 0; i < countOfDashes; i++)
        {
            Vector3 dashPoint = target.position + Quaternion.Euler(0f, 0f, Random.Range(-90f, 90f)) * Vector3.ClampMagnitude(target.position - transform.position, maxDistanceToTargetAfterDash);
            evilSpiritSounds.PlayDashSound();
            yield return transform.DOMove(dashPoint, dashDuration.Evaluate(hitpoints / maxHitpoints)).WaitForCompletion();
        }
        SetState(State.Hunt);
    }
    public void ReceiveDamage(float incomingDamage)
    {
        hitpoints -= incomingDamage;
    }
    public void Die()
    {
        GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
        {
            redKeyShard.GetComponent<ShardBehaviour>().owner = Owner.World;
            redKeyShard.transform.position = redKeyShardImage.transform.position;
            redKeyShard.transform.DOMoveY(transform.position.y, 0.25f).SetEase(Ease.InBack);
            G.enemiesDieStates.SetDieState(objectId);
            Destroy(gameObject);
        });
    }
    private void OnPlayerDied()
    {
        isAggro = false;
        hitpoints = maxHitpoints;
    }
    private void FlipSprite()
    {
        transform.rotation = Quaternion.Euler(0, movement.x > 0f ? 0 : 180, 0);
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[] { hitpoints, state, transform.position });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - hitpoints
        if(float.TryParse(dataToUnpack.data[0].ToString(), out var parsedHitpoints))
            hitpoints = parsedHitpoints;
        //data[1] - state
        if (Enum.TryParse(dataToUnpack.data[1].ToString(), out State parsedState))
            SetState(parsedState);
        //data[2] - position
        transform.position = ((JObject)dataToUnpack.data[2]).ToObject<Vector3>();
    }
}
