using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinalGatesBehaviour : MonoBehaviour, ISaveLoadObject
{
    [SerializeField] private float gatesChargeTime;
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

    private const byte NUMBER_OF_RUNES = 12;
    
    private byte shardsMask;
    private ShardSlot redShardSlot;
    private ShardSlot blueShardSlot;
    private ShardSlot greenShardSlot;
    
    private float currentBetweenRunesDelay;
    private float currentTimer;
    private int currentRune;
    private List<SpriteRenderer> sequence = new ();
    private SpriteRenderer onGatesRunesSprite;
    private Tween onGatesRunesTween;
    private Tween runesBlinkingSpeedUpTween;
    private AudioSource finalGatesAudio;
    private Tween audioFadeTween;
    private bool isCharging;
    private FinalGatesSounds finalGatesSounds;
    

    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
        redShardSlot = redShardSprite.GetComponent<ShardSlot>();
        blueShardSlot = blueShardSprite.GetComponent<ShardSlot>();
        greenShardSlot = greenShardSprite.GetComponent<ShardSlot>();
        onGatesRunesSprite = transform.Find("ActiveRunes").GetComponent<SpriteRenderer>();
        finalGatesSounds = GetComponent<FinalGatesSounds>();
        onGatesRunesTween = onGatesRunesSprite.DOFade(1f, 0.5f).SetAutoKill(false).Pause();
        currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(0f);
    }

    private void Start()
    {
        redShardSlot.OnGetShard += OnGetRedShard;
        blueShardSlot.OnGetShard += OnGetBlueShard;
        greenShardSlot.OnGetShard += OnGetGreenShard;
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
                if (isCharging && finalGatesAudio)
                {
                    float currentCycleDuration = Mathf.Max(currentBetweenRunesDelay * NUMBER_OF_RUNES, 0.01f);
                    finalGatesAudio.pitch = finalGatesAudio.clip.length / currentCycleDuration;
                    finalGatesAudio.volume = 10f / finalGatesAudio.pitch;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LightSideBehaviour _) && shardsMask == 0b111)
        {
            isCharging = true;
            onGatesRunesTween.Restart();
            if (!finalGatesAudio)
            {
                finalGatesAudio = finalGatesSounds.PlayChargingSound();
            }
            else
            {
                audioFadeTween?.Kill();
                finalGatesAudio.time = 0f;
                finalGatesAudio.volume = 1f;
            }
            runesBlinkingSpeedUpTween ??= DOVirtual.Float(0f, 1f, gatesChargeTime, value =>
            {
                currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(value);

            }).OnComplete(() =>
            {
                G.gameComplete.EndGame();
                finalGatesAudio.Stop();
                finalGatesAudio = null;
                currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(0f);
            });
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out LightSideBehaviour _) && shardsMask == 0b111)
        {
            isCharging = false;
            onGatesRunesTween.PlayBackwards();
            runesBlinkingSpeedUpTween?.Kill();
            runesBlinkingSpeedUpTween = null;
            if (finalGatesAudio)
            {
                audioFadeTween = DOVirtual.Float(1f, 0f, 0.5f, value =>
                {
                    finalGatesAudio.volume = value;
                }).OnComplete(() => finalGatesAudio = null).SetEase(Ease.OutQuart);
            }
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
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        float runesBlinkingSpeedUpTweenElapsedTime = isCharging ? runesBlinkingSpeedUpTween.Elapsed(false) : 0f;
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            shardsMask,
            currentRune,
            isCharging,
            runesBlinkingSpeedUpTweenElapsedTime
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - shardsMask
        if (byte.TryParse(dataToUnpack.data[0].ToString(), out var parsedShardsMask))
        {
            shardsMask = parsedShardsMask;
            if ((shardsMask & RED_BIT) != 0)
            {
                redShardSlot.MoveShardToSlot();
                redShardSlot.GetComponent<Collider2D>().enabled = false;
            }
            if ((shardsMask & BLUE_BIT) != 0)
            {
                blueShardSlot.MoveShardToSlot();
                blueShardSlot.GetComponent<Collider2D>().enabled = false;
            }
            if ((shardsMask & GREEN_BIT) != 0)
            {
                greenShardSlot.MoveShardToSlot();
                greenShardSlot.GetComponent<Collider2D>().enabled = false;
            }
        }
        //data[1] - currentRune
        if (int.TryParse(dataToUnpack.data[1].ToString(), out var parsedCurrentRune))
        {
            currentRune = parsedCurrentRune;
        }
        //data[2] - isCharging
        if (bool.TryParse(dataToUnpack.data[2].ToString(), out var parsedIsCharging))
        {
            isCharging = parsedIsCharging;
        }
        //data[3] - runesBlinkingSpeedUpTweenElapsedTime
        if (float.TryParse(dataToUnpack.data[3].ToString(), out var parsedElapsedTime))
        {
            if (isCharging)
            {
                runesBlinkingSpeedUpTween = DOVirtual.Float(parsedElapsedTime / gatesChargeTime, 1f, gatesChargeTime - parsedElapsedTime, value =>
                {
                    currentBetweenRunesDelay = betweenRunesDelayCurve.Evaluate(value);
                });
            }
        }
    }
}
