using UnityEngine;
using UnityEngine.AI;

public class WanderingClotMovement : ClotMovement
{
    [Header("Wandering movement")]
    [SerializeField] private float wanderingMoveSpeed;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private int wanderingPointsOnCircle;

    public override void SetMoveState(ClotBehaviour.State state)
    {
        currentState = state;
        switch (state)
        {
            case ClotBehaviour.State.Idle:
                target = transform;
                navMeshAgent.speed = wanderingMoveSpeed;
                break;
            case ClotBehaviour.State.Hunt:
                target = G.characters.lightSide.GetComponent<Transform>();
                navMeshAgent.speed = moveSpeed;
                break;
            case ClotBehaviour.State.Retreat:
                target = G.characters.equilibrium.GetComponent<Transform>();
                navMeshAgent.speed = moveSpeed;
                break;
        }
    }
    
    public override NavMeshPath GetOnCirclePositionForCurrentState()
    {
        switch (currentState)
        {
            case ClotBehaviour.State.Idle:
                return GetOnCircleNavMeshPosition(wanderingDistance, wanderingPointsOnCircle);
            case ClotBehaviour.State.Retreat:
                return GetOnCircleNavMeshPosition(retreatDistance,  retreatPointsOnCircle, true);
            default:
                return base.GetOnCirclePositionForCurrentState();
        }
    }
}
