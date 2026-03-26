using DG.Tweening;
using UnityEngine;

public class TorchAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField, Min(1)] private int framesPerSecond;

    private SpriteRenderer spriteRenderer;
    private float frameDuration;
    private float animationTimer;
    private int currentFrame;
    private Transform lightTransform;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        frameDuration = 1f / framesPerSecond;
        lightTransform = transform.Find("Light");
        lightTransform.DOScale(lightTransform.localScale * 0.95f, 0.25f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        animationTimer -= Time.deltaTime;
        if (animationTimer <= 0f)
        {
            spriteRenderer.sprite = animationSprites[currentFrame];
            currentFrame = (currentFrame + 1) % animationSprites.Length;
            animationTimer = frameDuration;
        }
    }
}
