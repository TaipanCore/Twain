using DG.Tweening;
using UnityEngine;

public class CircleLight : LightSource
{
    protected Tween currentAnim;
    protected delegate void VoidDelegate();
    protected VoidDelegate LowPriorityAnim;
    public override float range
    {
        get { return _range; }
        set
        {
            _range = value;
            currentAnim?.Kill();
            currentAnim = transform.DOScale(Vector3.one * (_range * 2f), 0.25f).OnComplete(() => LowPriorityAnim?.Invoke());
        }
    }
    private void OnEnable()
    {
        currentAnim?.Kill();
        currentAnim = transform.DOScale(transform.localScale, 0.8f).From(Vector3.zero).OnComplete(() => LowPriorityAnim?.Invoke());
    }
}
