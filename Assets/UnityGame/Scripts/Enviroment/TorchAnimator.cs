using DG.Tweening;
using UnityEngine;

public class TorchAnimator : SimpleAnimator
{
    private Transform lightTransform;

    protected override void Start()
    {
        base.Start();
        lightTransform = transform.Find("Light");
        lightTransform.DOScale(lightTransform.localScale * 0.95f, 0.25f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }
}
