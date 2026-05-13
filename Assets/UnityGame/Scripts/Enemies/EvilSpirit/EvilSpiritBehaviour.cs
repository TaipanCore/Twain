using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EvilSpiritBehaviour : MonoBehaviour, IDamageReceiver, IReactToFocusedLight, IAbleAggro
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
                steamParticles.Play();
            else
                steamParticles.Stop();
        }
    }
    public bool isAggro { get; set; }
    
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
    private GameObject redKeyShard;
    private float maxHitpoints;
    private Vector2 movement;
    private Vector3 previousPosition;
    private Animator animator;
    private State state;

    private void Start()
    {
        redEyeTrail = transform.Find("RedEye").GetComponent<TrailRenderer>();
        steamParticles = transform.Find("SteamParticles").GetComponent<ParticleSystem>();
        transform.Find("RedEye").GetComponent<RedEyeBehaviour>().damage = damage;
        redKeyShard = transform.Find("RedKeyShard").gameObject;
        animator = GetComponent<Animator>();
        maxHitpoints = hitpoints;
        dashTimer = timeInLightToDash;
        SetState(State.Idle);
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
        redKeyShard.SetActive(true);
        redEyeTrail.emitting = false;
        animator.SetTrigger(PlayIdle);
    }
    private void SetHuntSettings()
    {
        redKeyShard.SetActive(true);
        redEyeTrail.emitting = false;
        animator.SetTrigger(PlayHunt);
    }
    private void SetAttackSettings()
    {
        redKeyShard.SetActive(false);
        redEyeTrail.emitting = true;
        animator.SetTrigger(PlayAttack);
    }
    private void SetDashSettings()
    {
        redKeyShard.SetActive(false);
        redEyeTrail.emitting = true;
        int countOfDashes = hitpoints / maxHitpoints > 0.5f ? Random.Range(1, 3) : Random.Range(2, 4);
        StartCoroutine(DoDashes(countOfDashes));
        animator.SetTrigger(PlayDash);
    }

    private void SetDieSettings()
    {
        redKeyShard.SetActive(true);
        redEyeTrail.emitting = false;
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
            redKeyShard.GetComponent<Collider2D>().enabled = true;
            redKeyShard.transform.SetParent(null);
            redKeyShard.transform.rotation = new Quaternion();
            redKeyShard.transform.DOMoveY(transform.position.y, 0.25f).SetEase(Ease.InBack);
            Destroy(gameObject);
        });
    }
    private void FlipSprite()
    {
        transform.rotation = Quaternion.Euler(0, movement.x > 0f ? 0 : 180, 0);
    }
}
