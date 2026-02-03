using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSideMovement : PlayerMovement
{
    public float baseMoveSpeed;
    public float focusedMoveSpeed;

    private Animator animator;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    protected override void FlipCharacter()
    {
        float mouseX = MouseTracker.mousePosition.x - transform.position.x;
        if (animator.GetBool("IsFocused"))
        {
            animator.SetFloat("WalkAnimReverse", Mathf.Sign(movement.x * mouseX));
            movement.x = mouseX;
        }      
        if (movement.x != 0)
            sr.flipX = movement.x < 0f;
    }
}
