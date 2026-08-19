using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShrineOfBalanceBehaviour : MonoBehaviour, ISaveLoadObject
{
    [SerializeField, Min(1)] private int maxEtherCount;
    [SerializeField, Min(0f)] private float circleLightMaxRadius;
    [SerializeField] private AnimationCurve runesBlinkingIntensity;
    
    [Header("Hints")]
    [SerializeField] private ButtonsHints buttonHints;
    [SerializeField] private HintsTrigger hintsTrigger;


    private bool isCharged;
    
    private int _etherCount;
    public int etherCount
    {
        get => _etherCount;
        set
        {
            _etherCount = Mathf.Clamp(value, 0, maxEtherCount);
            if (!isCharged)
            {
                if (_etherCount == maxEtherCount)
                {
                    circleLight.SetRange(circleLightMaxRadius, 0.25f);
                    runesTween.Rewind();
                    runesSpriteRenderer.DOFade(1f, 0.1f);
                    runesParticleSystem.Play();
                    shrineOfBalanceSounds.PlayChargedSound();
                    isCharged = true;
                }
                else
                {
                   runesIntensity = (float)_etherCount / maxEtherCount;
                   circleLight.SetRange(runesIntensity * circleLightMaxRadius,0.25f);
                   runesTween.timeScale = runesBlinkingIntensity.Evaluate(runesIntensity);
                   if (!runesTween.IsPlaying())
                   {
                       runesTween.Play();
                   } 
                }
            }
            //Debug.Log($"{_etherCount}/{maxEtherCount}");
        }
    }
    
    private CircleLight circleLight;
    private SpriteRenderer runesSpriteRenderer;
    private ParticleSystem runesParticleSystem;
    private ShrineOfBalanceSounds shrineOfBalanceSounds;
    private float runesIntensity;
    private Tween runesTween;
    private Image equilibriumChargeBackground;
    
    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
        circleLight = transform.Find("CircleLight").GetComponent<CircleLight>();
        runesSpriteRenderer = transform.Find("Runes").GetComponent<SpriteRenderer>();
        shrineOfBalanceSounds = GetComponent<ShrineOfBalanceSounds>();
        runesParticleSystem = runesSpriteRenderer.transform.Find("BouncingUpRays").GetComponent<ParticleSystem>();
        runesTween = runesSpriteRenderer.DOFade(1f, 0.75f).SetLoops(-1, LoopType.Yoyo).Pause();
    }

    private void Start()
    {
        equilibriumChargeBackground = G.HUD.equilibriumCharge.transform.Find("Background").GetComponent<Image>();
        hintsTrigger.Initialize
        (
            () =>
            {
                if (isCharged && !G.characters.hasEquilibriumCharge)
                    buttonHints.ShowHint(ButtonsHints.BtnKey.E);
            },
            () => buttonHints.HideHint(ButtonsHints.BtnKey.E)
        );
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (G.input.interactiveBtnDown && !G.characters.hasEquilibriumCharge)
        {
            hintsTrigger.btnFirstActivated = true;
            G.characters.hasEquilibriumCharge = true;
            equilibriumChargeBackground.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0f), 1.5f, 0, 0f).SetEase(Ease.OutCubic);
            equilibriumChargeBackground.DOFade(1f, 0f);
            isCharged = false;
            runesParticleSystem.Stop();
            shrineOfBalanceSounds.PlayGetChargeSound();
            _etherCount = 0;
            runesSpriteRenderer.DOFade(0f, 0f);
            circleLight.SetRange(0f, 0.25f);
        }
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        bool interactHintActivated = hintsTrigger.btnFirstActivated;
        bool mapHintActivated = GetComponent<MapOpenerAndCloser>().hintsTrigger.btnFirstActivated;
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            etherCount,
            interactHintActivated,
            mapHintActivated
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - etherCount
        if(int.TryParse(dataToUnpack.data[0].ToString(), out var parsedEtherCount)) 
            etherCount = parsedEtherCount;
        //data[1] - interactHintActivated
        if(bool.TryParse(dataToUnpack.data[1].ToString(), out var parsedInteractHintActivated)) 
            hintsTrigger.btnFirstActivated = parsedInteractHintActivated;
        //data[2] - mapHintActivated
        if(bool.TryParse(dataToUnpack.data[2].ToString(), out var parsedMapHintActivated))
            GetComponent<MapOpenerAndCloser>().hintsTrigger.btnFirstActivated = parsedMapHintActivated;
    }
}
