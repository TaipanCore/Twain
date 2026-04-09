using UnityEngine;

public class SquidEyeAnimator : SimpleAnimator
{
    [SerializeField] private float minBlinkDelay;
    [SerializeField] private float maxBlinkDelay;

    private float blinkTimer;

    protected override void Start()
    {
        base.Start();
        blinkTimer = Random.Range(minBlinkDelay, maxBlinkDelay);
    }
    protected override void Update()
    {
        base.Update();
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0)
        {
            Restart();
            blinkTimer = Random.Range(minBlinkDelay, maxBlinkDelay);
        }
    }
}
