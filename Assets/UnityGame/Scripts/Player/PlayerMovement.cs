using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    [HideInInspector] public float currentSpeed;

    protected Rigidbody2D rb;
    protected Vector2 movementVector;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void FixedUpdate()
    {
        if (G.characters.currentCharacter == gameObject)
        {
            movementVector = G.input.movement;
            rb.MovePosition(rb.position + movementVector * (moveSpeed * Time.fixedDeltaTime));
            currentSpeed = (movementVector * moveSpeed).magnitude;
            FlipCharacter();
        }
        else
        {
            currentSpeed = 0f;
        }
    }
    protected virtual void FlipCharacter()
    {
        if (movementVector.x != 0)
            if (movementVector.x < 0f)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else
                transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
