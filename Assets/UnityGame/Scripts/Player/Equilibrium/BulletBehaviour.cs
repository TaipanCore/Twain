using System;
using DG.Tweening;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float acceleration;
    [SerializeField] private float stunTime;
    [SerializeField] private Rigidbody2D rb;

    
    private Vector2 direction;
    private Tween destroyTween;
    private Action OnBulletDestroyed;
    private void Start()
    {
        destroyTween ??= DOVirtual.DelayedCall(lifeTime, () => Destroy(gameObject), false);
        direction = transform.right;
    }
    private void FixedUpdate()
    {
        rb.AddForce(direction * acceleration);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IStunnable stunnable) && collision.gameObject.IsInLayerMask(G.enemyMask))
        {
            stunnable.ApplyStun(stunTime);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        destroyTween?.Kill();
        OnBulletDestroyed?.Invoke();
    }

    public void Initialize(float stunTime, Action OnBulletDestroyed)
    {
        this.stunTime = stunTime;
        this.OnBulletDestroyed = OnBulletDestroyed;
    }

    public BulletData PackBulletData()
    {
        return new BulletData(transform.position, transform.rotation, rb.velocity, lifeTime - destroyTween.Elapsed(false));
    }

    public void UnpackBulletData(BulletData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        rb.velocity = data.velocity;
        destroyTween ??= DOVirtual.DelayedCall(data.remainingLifetime, () => Destroy(gameObject), false);
    }

    [Serializable]
    public class BulletData
    {
        public BulletData(Vector3 position, Quaternion rotation, Vector2 velocity, float remainingLifetime)
        {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.remainingLifetime = remainingLifetime;
        }
        
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public float remainingLifetime;
    }
}
