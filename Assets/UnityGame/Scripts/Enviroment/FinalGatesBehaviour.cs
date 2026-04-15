using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinalGatesBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve betweenRunesDelayCurve;
    [Header("Red shard")]
    [SerializeField] private SpriteRenderer redShardSprite;
    [SerializeField] private SpriteRenderer[] redShardRunes;
    [Header("Blue shard")]
    [SerializeField] private SpriteRenderer blueShardSprite;
    [SerializeField] private SpriteRenderer[] blueShardRunes;
    [Header("Green shard")]
    [SerializeField] private SpriteRenderer greenShardSprite;
    [SerializeField] private SpriteRenderer[] greenShardRunes;

    private const byte RED_BIT = 0b001;
    private const byte BLUE_BIT = 0b010;
    private const byte GREEN_BIT = 0b100;
    
    private byte shardsMask;
    private ShardSlot redShardSlot;
    private ShardSlot blueShardSlot;
    private ShardSlot greenShardSlot;
    
    private float currentBetweenRunesDelay;
    private float currentTimer;
    private int currentRune;
    private List<SpriteRenderer> sequence = new List<SpriteRenderer>();
    private SpriteRenderer onGatesRunesSprite;
    private Tween onGatesRunesTween;
    private Tween runesBlinkingSpeedUpTween;
    
    private void Start()
    {
        redShardSlot = redShardSprite.GetComponent<ShardSlot>();
        redShardSlot.OnGetShard += OnGetRedShard;
        blueShardSlot = blueShardSprite.GetComponent<ShardSlot>();
        blueShardSlot.OnGetShard += OnGetBlueShard;
        greenShardSlot = greenShardSprite.GetComponent<ShardSlot>();
        greenShardSlot.OnGetShard += OnGetGreenShard;
        onGatesRunesSprite = transform.Find("ActiveRunes").GetComponent<SpriteRenderer>();
        onGatesRunesTween = onGatesRunesSprite.DOFade(1f, 0.75f).SetAutoKill(false).Pause();
        currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(0f);
    }
    private void Update()
    {
        if (sequence.Count > 0)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                sequence[currentRune].DOFade(1f, currentBetweenRunesDelay).SetLoops(2, LoopType.Yoyo);
                currentRune++;
                currentTimer = currentBetweenRunesDelay;
                if (currentRune == sequence.Count)
                {
                    currentRune = 0;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LightSideBehaviour _) && shardsMask == 0b111)
        {
            onGatesRunesTween.Restart();
            runesBlinkingSpeedUpTween = DOVirtual.Float(0f, 1f, 20f, value =>
            {
                currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(value);
            });
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LightSideBehaviour _) && shardsMask == 0b111)
        {
            onGatesRunesTween.PlayBackwards();
            runesBlinkingSpeedUpTween.Kill();
            currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(0f);
        }
    }

    private void OnGetRedShard()
    {
        shardsMask |= 1 << 0;
        sequence = GetSequence();
        redShardSlot.OnGetShard -= OnGetRedShard;
    }
    private void OnGetBlueShard()
    {
        shardsMask |= 1 << 1;
        sequence = GetSequence();
        blueShardSlot.OnGetShard -= OnGetBlueShard;
    }
    private void OnGetGreenShard()
    {
        shardsMask |= 1 << 2;
        sequence = GetSequence();
        greenShardSlot.OnGetShard -= OnGetGreenShard;
    }
    private List<SpriteRenderer> GetSequence()
    {
        List<SpriteRenderer> newSequence = new List<SpriteRenderer>();
        if ((shardsMask & RED_BIT) != 0)
        {
            newSequence.Add(redShardSprite);
            newSequence.AddRange(redShardRunes);
        }
        else
        {
            return newSequence;
        }
        if ((shardsMask & BLUE_BIT) != 0)
        {
            newSequence.Add(blueShardSprite);
            newSequence.AddRange(blueShardRunes);
        }
        else
        {
            return newSequence;
        }
        if ((shardsMask & GREEN_BIT) != 0)
        {
            newSequence.Add(greenShardSprite);
            newSequence.AddRange(greenShardRunes);
        }
        return newSequence;
    }
}
