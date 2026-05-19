using UnityEngine;

public class WanderingClotMovement : ClotMovement
{
    [Header("Wandering movement")]
    [SerializeField] private float wanderingMoveSpeed;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private int wanderingPointsOnCircle;
    
    public Vector3 GetWanderingPosition()
    {
        return GetOnCircleNavMeshPosition(wanderingDistance, wanderingPointsOnCircle);
    }

    public void SetWanderingMoveSpeed()
    {
        navMeshAgent.speed = wanderingMoveSpeed;
    }
}
