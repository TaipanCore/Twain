using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClotBehaviour : MonoBehaviour
{
    private enum Behaviour
    {
        Idle,
        Hunt,
        Retreat
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float retreatDistance;

    private NavMeshAgent agent;
    private Behaviour behaviour;
    private Vector3 target;

    private void Start()
    {
        SetupNavMeshAgent();
        SetBehaviour(Behaviour.Hunt);
    }
    private void FixedUpdate()
    {
        switch (behaviour)
        {
            case Behaviour.Idle:
                Idle();
                break;                
            case Behaviour.Hunt:
                Hunt();
                break;
            case Behaviour.Retreat:
                Retreat();
                break;   
        }
    }

    private void SetBehaviour(Behaviour newBehaviour)
    {
        if (behaviour != newBehaviour)
        {
            behaviour = newBehaviour;
            switch (behaviour)
            {
                case Behaviour.Idle:
                    SetIdleSettings();
                    break;
                case Behaviour.Hunt:
                    SetHuntSettings();
                    break;
                case Behaviour.Retreat:
                    SetRetreatSettings();
                    break;
            }
        }
    }
    private void SetIdleSettings()
    {

    }
    private void SetHuntSettings()
    {

    }
    private void SetRetreatSettings()
    {

    }
    private void Idle()
    {

    }
    private void Hunt()
    {
        if (GameManager.isUnited)
        {
            SetBehaviour(Behaviour.Retreat);
        }
        target = GameManager.LightSide.transform.position;
        agent.SetDestination(target);
    }
    private void Retreat()
    {
        if (!GameManager.isUnited)
        {
            SetBehaviour(Behaviour.Hunt);
        }
        target = GameManager.Equilibrium.transform.position;
        agent.SetDestination(GetRetreatPosition());
    }
    private Vector3 GetRetreatPosition()
    {
        Vector3 retreatVector = (transform.position - target).normalized;
        if (retreatVector == Vector3.zero)
            retreatVector = Random.onUnitSphere;
        NavMeshHit hit;
        NavMesh.SamplePosition(target + retreatVector * retreatDistance, out hit, 2 * retreatDistance, NavMesh.AllAreas);
        return hit.position;
    }
    private void SetupNavMeshAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target, retreatDistance);
    }
}
