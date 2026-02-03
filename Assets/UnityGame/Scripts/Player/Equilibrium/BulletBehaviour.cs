using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float acceleration;
    [SerializeField] private float stunTime;

    private Rigidbody2D rb;
    private Vector2 direction;
    private void Start()
    {
        Destroy(gameObject, lifeTime);
        rb = GetComponent<Rigidbody2D>();
        direction = transform.right;
    }
    private void FixedUpdate()
    {
        rb.AddForce(direction * acceleration);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IStunnable stunnable) && collision.gameObject.IsInLayerMask(GameManager.enemyMask))
        {
            stunnable.ApplyStun(stunTime);
        }
        Destroy(gameObject);
    }
}
