using DG.Tweening;
using UnityEngine;

public class CircleLight : LightSource
{
    private Tween currentAnim;
    public override float range
    {
        get { return _range; }
        set
        {
            _range = value;
            currentAnim?.Kill();
            currentAnim = transform.DOScale(Vector3.one * (_range * 2f), 0.25f);
        }
    }
    private void OnEnable()
    {
        currentAnim?.Kill();
        currentAnim = transform.DOScale(transform.localScale, 0.8f).From(Vector3.zero);
    }
}
