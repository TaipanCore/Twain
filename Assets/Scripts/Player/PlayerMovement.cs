using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] GameManager gameManager;

    private Vector3 movement;
    private void Update()
    {
        if (gameManager.currentCharacter == gameObject)
        {
            float horizontalMove = Input.GetAxis("Horizontal");
            float verticalMove = Input.GetAxis("Vertical");
            movement = new Vector3(horizontalMove, verticalMove, 0f);           
        }
    }
    private void FixedUpdate()
    {
        if (gameManager.currentCharacter == gameObject)
        {
            transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
