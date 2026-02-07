using System.Collections;
using UnityEngine;

public class ClotBehaviour : MonoBehaviour, IDamageDealer, IDamageReceiver, IAbleAggro, IStunnable
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
    private WaitForSeconds invulnerableDelay;
    public float invulnerableTime
    {
        get => _invulnerableTime;
        set => _invulnerableTime = value;
    }
    public bool isInvulnerable { get; set; } = false;
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

    private ClotMovement movement;
    private Animator animator;
    private State state;

    private Coroutine retreatCoroutine;

    private void Start()
    {
        SetState(State.Idle);
        movement = GetComponent<ClotMovement>();
        animator =  GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
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
        if (state != newState)
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
    }
    public State GetState()
    {
        return state;
    }
    private void SetIdleSettings()
    {
        movement.target = transform.position;
    }
    private void SetHuntSettings()
    {

    }
    private void SetRetreatSettings()
    {
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
        if (GameManager.isUnited)
        {
            SetState(State.Retreat);
        }
        movement.target = GameManager.LightSide.transform.position;
        movement.navMeshAgent.SetDestination(movement.target);
    }
    private void Retreat()
    {
        if (!GameManager.isUnited)
        {
            StopCoroutine(retreatCoroutine);
            retreatCoroutine = null;
            SetState(State.Hunt);
        }
        movement.target = GameManager.Equilibrium.transform.position;
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
        if (!isInvulnerable)
        {
            hitpoints -= damage;
            StartCoroutine(GiveInvulnerability());
        }
    }
    public IEnumerator GiveInvulnerability()
    {
        isInvulnerable = true;
        yield return invulnerableDelay;
        isInvulnerable = false;
    }
    public void Die()
    {
        Destroy(gameObject);
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
