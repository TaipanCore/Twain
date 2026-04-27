using DG.Tweening;
using UnityEngine;

public class CircleLight : LightSource
{
    protected Tween currentAnim;
    protected delegate void VoidDelegate();
    protected VoidDelegate LowPriorityAnim;
    
    private void Awake()
    {
        range = transform.localScale.x / 2f;
    }
    private void OnEnable()
    {
        currentAnim?.Kill();
        currentAnim = transform.DOScale(Vector3.one * (range * 2f), 0.25f).From(Vector3.zero).OnComplete(() => LowPriorityAnim?.Invoke());
    }

    public override void SetRange(float newRange)
    {
        range = newRange;
        transform.localScale = Vector3.one * (range * 2f);
    }
    public void SetRange(float newRange, float duration)
    {
        range = newRange;
        currentAnim?.Kill();
        currentAnim = transform.DOScale(Vector3.one * (range * 2f), duration).OnComplete(() => LowPriorityAnim?.Invoke());
    }
    public override float GetRange()
    {
        return range;
    }
}
