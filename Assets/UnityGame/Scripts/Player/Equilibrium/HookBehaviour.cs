using DG.Tweening;
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
            Time.timeScale = 0.1f;
            GameManager.mainCamera.transform.DOShakePosition(0.25f, 0.15f).SetEase(Ease.OutBounce).OnComplete(() => Time.timeScale = 1f);
            DealDamage(damage, damageReceiver);
        }
    }
    public void DealDamage(float damage, IDamageReceiver target)
    {
        target.ReceiveDamage(damage);
    }
}
