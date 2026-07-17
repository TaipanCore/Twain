using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShrineOfBalanceBehaviour : MonoBehaviour, ISaveLoadObject
{
    [SerializeField, Min(1)] private int maxEtherCount;
    [SerializeField, Min(0f)] private float circleLightMaxRadius;
    [SerializeField] private AnimationCurve runesBlinkingIntensity;

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
    private float runesIntensity;
    private Tween runesTween;
    private Image equilibriumChargeBackground;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        circleLight = transform.Find("CircleLight").GetComponent<CircleLight>();
        runesSpriteRenderer = transform.Find("Runes").GetComponent<SpriteRenderer>();
        runesParticleSystem = runesSpriteRenderer.transform.Find("BouncingUpRays").GetComponent<ParticleSystem>();
        runesTween = runesSpriteRenderer.DOFade(1f, 0.75f).SetLoops(-1, LoopType.Yoyo).Pause();
        equilibriumChargeBackground = G.HUD.equilibriumCharge.transform.Find("Background").GetComponent<Image>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (G.input.interactiveBtnDown && collision.gameObject.CompareTag("Player") && !G.characters.hasEquilibriumCharge)
        {
            G.characters.hasEquilibriumCharge = true;
            equilibriumChargeBackground.DOFade(1f, 0f);
            isCharged = false;
            runesParticleSystem.Stop();
            _etherCount = 0;
            runesSpriteRenderer.DOFade(0f, 0f);
            circleLight.SetRange(0f, 0.25f);
        }
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[] { etherCount });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - etherCount
        if(int.TryParse(dataToUnpack.data[0].ToString(), out var parsedEtherCount)) 
            etherCount = parsedEtherCount;
    }
}
