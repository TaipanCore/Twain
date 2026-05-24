using UnityEngine;
using UnityEngine.AI;

public class ClotMovement : MonoBehaviour
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int RunAnimMultiplier = Animator.StringToHash("RunAnimMultiplier");
    
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float turnAroundSpeedThreshold;
    [SerializeField] protected float retreatDistance;
    [SerializeField] protected int retreatPointsOnCircle;
    
    [HideInInspector] public Transform target;
    [HideInInspector] public bool isInPanic;
    [SerializeField] public NavMeshAgent navMeshAgent;
    
    private ClotBehaviour clotBehaviour;
    private Transform objectTransform;
    private Animator animator;
    private Vector2 movementVector;
    private float currentSpeed;
    protected ClotBehaviour.State currentState;

    private void Start()
    {
        clotBehaviour = GetComponent<ClotBehaviour>();
        objectTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        SetupNavMeshAgent();
    }
    private void FixedUpdate()
    {
        movementVector = navMeshAgent.velocity;
        currentSpeed = movementVector.magnitude;
        animator.SetFloat(MovSpeed, currentSpeed);
        animator.SetFloat(RunAnimMultiplier, movementVector.x < 0f != Mathf.Approximately(Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y), -1) ? -currentSpeed / moveSpeed : currentSpeed / moveSpeed);
        FlipCharacter();
    }

    public virtual void SetMoveState(ClotBehaviour.State state)
    {
        currentState = state;
        switch (state)
        {
            case ClotBehaviour.State.Idle:
                target = transform;
                navMeshAgent.speed = 0f;
                break;
            case ClotBehaviour.State.Hunt:
                target = GameManager.lightSide.GetComponent<Transform>();
                navMeshAgent.speed = moveSpeed;
                break;
            case ClotBehaviour.State.Retreat:
                target = GameManager.equilibrium.GetComponent<Transform>();
                navMeshAgent.speed = moveSpeed;
                break;
        }
    }
    
    private void SetupNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    public virtual NavMeshPath GetOnCirclePositionForCurrentState()
    {
        return GetOnCircleNavMeshPosition(retreatDistance,  retreatPointsOnCircle, true);
    }
    
    public NavMeshPath GetOnCircleNavMeshPosition(float circleRadius, int pointsOnCircle, bool needShortestPath = false)
    {
        NavMeshPath validPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.position + (Vector3)Random.insideUnitCircle * (circleRadius / 1.5f), NavMesh.AllAreas, validPath);
        if (!isInPanic)
            isInPanic = true;
        float minPathLength = float.MaxValue;
        float degreesStep = 360f / pointsOnCircle;
        Vector2 movingVector = (target != transform ? (Vector2)(transform.position - target.position) : Random.insideUnitCircle).normalized;
        for (int i = 0; i < pointsOnCircle; i++)
        {
            movingVector = Quaternion.Euler(0, 0, degreesStep) * movingVector;
            Vector3 movingPosition = target.position + (Vector3)movingVector * circleRadius;
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, movingPosition, NavMesh.AllAreas, path))
            {
                if (isInPanic)
                    isInPanic = false;
                if (needShortestPath)
                {
                    float pathLength = Utils.GetPathLength(path);
                    if (pathLength < minPathLength)
                    {
                        validPath = path;
                        minPathLength = pathLength;              
                    }
                }
                else
                {
                    return path;
                }
            }
        }
        return validPath;
    }
    private void FlipCharacter()
    {
        if (movementVector.x != 0)
        {
            if (clotBehaviour.GetState() == ClotBehaviour.State.Retreat && currentSpeed < turnAroundSpeedThreshold)
            {
                transform.rotation = Quaternion.Euler(0, objectTransform.position.x > target.position.x ? 180 : 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, movementVector.x < 0f ? 180 : 0, 0);
            }
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position, retreatDistance);
    }*/
}
