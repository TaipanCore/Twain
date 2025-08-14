using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (GameManager.currentCharacter == gameObject)
        {
            rb.MovePosition(rb.position + InputManager.movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
