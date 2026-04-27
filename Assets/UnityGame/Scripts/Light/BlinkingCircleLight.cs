using DG.Tweening;
using UnityEngine;

public class BlinkingCircleLight : CircleLight
{
    [SerializeField] private float blinkingRadiusMultiplier;
    [SerializeField] private float blinkDuration;
    [SerializeField] private Ease blinkEase;

    private void Start()
    {
        LowPriorityAnim = BlinkingAnim;
    }
    private void BlinkingAnim()
    {
        currentAnim = transform.DOScale(Vector3.one * (range * 2f * blinkingRadiusMultiplier), blinkDuration).SetLoops(-1, LoopType.Yoyo).SetEase(blinkEase);
    }
}
