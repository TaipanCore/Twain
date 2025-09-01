using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float currentSpeed;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Vector2 movement;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();       
    }
    private void FixedUpdate()
    {
        if (GameManager.currentCharacter == gameObject)
        {
            movement = InputManager.movement;
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            currentSpeed = (movement * moveSpeed).magnitude;
            FlipSprite();
        }
    }
    protected virtual void FlipSprite()
    {
        if (movement.x != 0)
            sr.flipX = movement.x < 0f;
    }
}
