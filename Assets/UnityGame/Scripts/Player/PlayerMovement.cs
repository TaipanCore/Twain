using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    [HideInInspector] public float currentSpeed;

    protected Rigidbody2D rb;
    protected Vector2 movement;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void FixedUpdate()
    {
        if (GameManager.currentCharacter == gameObject)
        {
            movement = InputManager.movement;
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
            currentSpeed = (movement * moveSpeed).magnitude;
            FlipCharacter();
        }
        else
        {
            currentSpeed = 0f;
        }
    }
    protected virtual void FlipCharacter()
    {
        if (movement.x != 0)
            if (movement.x < 0f)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
