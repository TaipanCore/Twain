using UnityEngine;

public class RedEyeBehaviour : MonoBehaviour, IDamageDealer
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
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageReceiver damageReceiver) && other.gameObject.IsInLayerMask(GameManager.playerMask))
        {
            DealDamage(damage, damageReceiver);
        }
    }
    public void DealDamage(float damage, IDamageReceiver damagedTarget)
    {
        damagedTarget.ReceiveDamage(damage);
    }
}
