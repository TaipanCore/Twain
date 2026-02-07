using UnityEngine;

public class LightSideMovement : PlayerMovement
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int IsFocused = Animator.StringToHash("IsFocused");
    private static readonly int WalkAnimReverse = Animator.StringToHash("WalkAnimReverse");
    
    public float baseMoveSpeed;
    public float focusedMoveSpeed;

    private Animator animator;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (GameManager.currentCharacter == gameObject)
            animator.SetFloat(MovSpeed, currentSpeed);
    }
    protected override void FlipCharacter()
    {
        float mouseX = MouseTracker.mousePosition.x - transform.position.x;
        if (animator.GetBool(IsFocused))
        {
            animator.SetFloat(WalkAnimReverse, Mathf.Sign(movement.x * mouseX));
            movement.x = mouseX;
        }      
        if (movement.x != 0)
            spriteRenderer.flipX = movement.x < 0f;
    }
}
