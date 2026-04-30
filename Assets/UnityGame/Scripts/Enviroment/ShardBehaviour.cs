using UnityEngine;

public class ShardBehaviour : MonoBehaviour
{
    private CapsuleCollider2D takeCollider;

    private void Start()
    {
        takeCollider = GetComponent<CapsuleCollider2D>();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (InputManager.interactiveBtnDown)
        {
            if (other.gameObject.IsInLayerMask(GameManager.playerMask))
            {
                takeCollider.enabled = false;
                GameManager.inventory.Add(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}
