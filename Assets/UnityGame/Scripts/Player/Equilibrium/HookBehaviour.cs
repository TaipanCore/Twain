using UnityEngine;

public class HookBehaviour : MonoBehaviour, IDamageDealer
{
    private float _damage;
    public float damage
    {
        get => _damage;
        set
        {
            _damage = value;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageReceiver damageReceiver) && other.gameObject.IsInLayerMask(GameManager.enemyMask))
        {
            DealDamage(damage, damageReceiver);
        }
    }
    public void DealDamage(float damage, IDamageReceiver target)
    {
        target.ReceiveDamage(damage);
    }
}
