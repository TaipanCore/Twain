using UnityEngine;
using UnityEngine.AI;

public class ClotMovement : MonoBehaviour
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int RunAnimMultiplier = Animator.StringToHash("RunAnimMultiplier");
    
    public float moveSpeed;
    public float currentSpeed;
    public float turnAroundSpeedThreshold;
    public float retreatDistance;
    public int pointsOnCircle;
    [HideInInspector] public Vector3 target;
    [HideInInspector] public bool isInPanic;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    
    private ClotBehaviour clotBehaviour;
    private SpriteRenderer spriteRenderer;
    private Transform objectTransform;
    private Animator animator;
    private Vector2 movementVector;

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
        animator.SetFloat(RunAnimMultiplier, movementVector.x < 0f != spriteRenderer.flipX ? -currentSpeed / moveSpeed : currentSpeed / moveSpeed);
        FlipCharacter();
    }
    private void SetupNavMeshAgent()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.speed = moveSpeed;
    }
    public Vector3 GetRetreatPosition()
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
    private void FlipCharacter()
    {
        if (movementVector.x != 0)
            if (clotBehaviour.GetState() == ClotBehaviour.State.Retreat && currentSpeed < turnAroundSpeedThreshold)
                spriteRenderer.flipX = objectTransform.position.x > target.x;
            else
                spriteRenderer.flipX = movementVector.x < 0f;
    }
}
