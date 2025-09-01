using NavMeshPlus.Extensions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ClotBehaviour : MonoBehaviour, IDamageDealer, IDamageReceiver, IAbleAggro
{
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

    [SerializeField] private float moveSpeed;
    [SerializeField] private float retreatDistance;
    [SerializeField] private int pointsOnCircle;

    private NavMeshAgent agent;
    private State state;
    private Vector3 target;

    private Coroutine retreatCoroutine;
    private bool isInPanic;

    private void Start()
    {
        SetupNavMeshAgent();
        SetState(State.Idle);
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
        target = transform.position;
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
        target = GameManager.LightSide.transform.position;
        agent.SetDestination(target);
    }
    private void Retreat()
    {
        if (!GameManager.isUnited)
        {
            StopCoroutine(retreatCoroutine);
            retreatCoroutine = null;
            SetState(State.Hunt);
        }
        target = GameManager.Equilibrium.transform.position;
    }
    private IEnumerator RetreatCoroutine()
    {
        while (true)
        {
            if (!isInPanic || (isInPanic && !Utils.IsAgentMoving(agent)))
            {
                agent.SetDestination(GetRetreatPosition());
            }
            yield return null;  
        }
    }
    private Vector3 GetRetreatPosition()
    {
        Vector3 nearestPosition = target + (Vector3)Random.insideUnitCircle * (retreatDistance / 1.5f);
        if (!isInPanic)
            isInPanic = true;
        float minPathLength = float.MaxValue;
        float degreesStep = 360f / pointsOnCircle;
        Vector2 retreatVector = (transform.position - target).normalized;
        for (int i = 0; i < pointsOnCircle; i++)
        {
            retreatVector = Quaternion.Euler(0, 0, degreesStep) * retreatVector;
            Vector3 retreatPosition = target + (Vector3)retreatVector * retreatDistance;
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, retreatPosition, NavMesh.AllAreas, path))
            {
                float pathLength = Utils.GetPathLength(path);
                if (pathLength < minPathLength)
                {
                    nearestPosition = retreatPosition;
                    minPathLength = pathLength;              
                }
                if (isInPanic)
                    isInPanic = false;
            }
        }
        return nearestPosition;
    }
    private void SetupNavMeshAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }
    public void ReceiveDamage(float damage)
    {
        hitpoints -= damage;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    /*
    private void OnDrawGizmos()
    {      
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target, retreatDistance);
    }
    */
}
