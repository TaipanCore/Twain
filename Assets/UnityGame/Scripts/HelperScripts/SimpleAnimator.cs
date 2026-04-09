using UnityEngine;

public class SimpleAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField, Min(1)] private int framerate;
    [SerializeField] private bool loopAnimation;
    [SerializeField] private bool playOnStart;
    
    private SpriteRenderer spriteRenderer;
    protected int currentFrame;
    private float frameDuration;
    private float animationTimer;
    private bool isPlaying;
    
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        frameDuration = 1.0f / framerate;
        if (playOnStart)
            Restart();
    }
    protected virtual void Update()
    {
        if (isPlaying)
        {
            animationTimer -= Time.deltaTime;
            if (animationTimer <= 0f)
            {
                FrameActions();
                currentFrame++;
                animationTimer = frameDuration;
                if (currentFrame == animationSprites.Length)
                {
                    currentFrame = 0;
                    if (!loopAnimation)
                        Stop();
                }
            }
        }
    }

    public virtual void SetFramerate(int frameRate)
    {
        framerate = frameRate;
        frameDuration = 1.0f / framerate;
    }

    public virtual int GetFramerate()
    {
        return framerate;
    }
    public virtual void Restart()
    {
        currentFrame = 0;
        animationTimer = 0;
        isPlaying = true;
    }
    public virtual void Pause()
    {
        isPlaying = false;
    }
    public virtual void Resume()
    {
        isPlaying = true;
    }
    public virtual void Stop()
    {
        isPlaying = false;
        currentFrame = 0;
        animationTimer = 0;
    }

    protected virtual void FrameActions()
    {
        spriteRenderer.sprite = animationSprites[currentFrame];
    }
}

