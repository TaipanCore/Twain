using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float currentSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        if (GameManager.currentCharacter == gameObject)
        {
            Vector2 movement = InputManager.movement;
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
            currentSpeed = (movement * moveSpeed).magnitude;
            if (movement.x != 0)
                sr.flipX = movement.x < 0f;
        }
    }
}
