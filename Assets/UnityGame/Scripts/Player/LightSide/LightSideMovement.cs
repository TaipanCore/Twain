using UnityEngine;

public class LightSideMovement : PlayerMovement
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int IsFocused = Animator.StringToHash("IsFocused");
    private static readonly int WalkAnimReverse = Animator.StringToHash("WalkAnimReverse");
    
    public float baseMoveSpeed;
    public float focusedMoveSpeed;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        animator.SetFloat(MovSpeed, currentSpeed);
    }
    protected override void FlipCharacter()
    {
        float mouseX = MouseTracker.mousePosition.x - transform.position.x;
        if (animator.GetBool(IsFocused))
        {
            animator.SetFloat(WalkAnimReverse, Mathf.Sign(movementVector.x * mouseX));
            movementVector.x = mouseX;
        }      
        if (movementVector.x != 0)
            spriteRenderer.flipX = movementVector.x < 0f;
    }
}
