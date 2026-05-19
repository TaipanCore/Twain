using UnityEngine;
using UnityEngine.AI;

public class ClotMovement : MonoBehaviour
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int RunAnimMultiplier = Animator.StringToHash("RunAnimMultiplier");
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnAroundSpeedThreshold;
    [SerializeField] private float retreatDistance;
    [SerializeField] private int retreatPointsOnCircle;
    
    [HideInInspector] public Transform target;
    [HideInInspector] public bool isInPanic;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    
    private ClotBehaviour clotBehaviour;
    private SpriteRenderer spriteRenderer;
    private Transform objectTransform;
    private Animator animator;
    private Vector2 movementVector;
    private float currentSpeed;

    private void Start()
    {
        clotBehaviour = GetComponent<ClotBehaviour>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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
    
    private void SetupNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }
    
    public void SetMoveSpeed()
    {
        navMeshAgent.speed = moveSpeed;
    }

    public Vector3 GetRetreatPosition()
    {
        return GetOnCircleNavMeshPosition(retreatDistance,  retreatPointsOnCircle);
    }
    
    protected Vector3 GetOnCircleNavMeshPosition(float circleRadius, int pointsOnCircle)
    {
        Vector3 validPosition = target.position + (Vector3)Random.insideUnitCircle * (circleRadius / 1.5f);
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
                float pathLength = Utils.GetPathLength(path);
                if (pathLength < minPathLength)
                {
                    validPosition = movingPosition;
                    minPathLength = pathLength;              
                }
                if (isInPanic)
                    isInPanic = false;
            }
        }
        return validPosition;
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
}
