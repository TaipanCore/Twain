using DG.Tweening;
using UnityEngine;

public class FocusedLight : LightSource
{
    private void Awake()
    {
        range = transform.localScale.x;
    }
    private void OnEnable()
    {
        transform.DOScale(Vector3.one * range, 0.8f).From(Vector3.zero);
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IEtherContainer etherContainer))
        {
            etherContainer.SpawnEtherParticle();
        }
    }
    
    public override void SetRange(float newRange)
    {
        range = newRange;
        transform.localScale = Vector3.one * range;
    }
    public void SetRange(float newRange, float duration)
    {
        range = newRange;
        transform.DOScale(Vector3.one * range, duration).From(Vector3.zero);
    }
    public override float GetRange()
    {
        return range;
    }
}
