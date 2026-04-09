using DG.Tweening;
using UnityEngine;

public class ShrineOfBalanceBehaviour : MonoBehaviour
{
    [SerializeField, Min(0f)] private float uniteDistance;
    [SerializeField, Min(0f)] private float timeInEquilibriumForm;
    [SerializeField, Min(1)] private int maxEtherCount;
    [SerializeField, Min(0f)] private float circleLightMaxRadius;
    [SerializeField] private AnimationCurve runesBlinkingIntensity;

    [HideInInspector] public bool isCharged;
    
    private int _etherCount;
    public int etherCount
    {
        get
        {
            return _etherCount;
        }
        set
        {
            _etherCount = Mathf.Clamp(value, 0, maxEtherCount);
            if (!isCharged)
            {
                if (_etherCount == maxEtherCount)
                {
                    circleLight.range = circleLightMaxRadius;
                    runesTween.Rewind();
                    runesSpriteRenderer.DOFade(1f, 0.1f);
                    runesParticleSystem.Play();
                    isCharged = true;
                }
                else
                {
                   runesIntensity = (float)_etherCount / maxEtherCount;
                   circleLight.range = runesIntensity * circleLightMaxRadius;
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
    private void Awake()
    {
        GameManager.shrineOfBalance = gameObject;
    }

    private void Start()
    {
        circleLight = transform.Find("CircleLight").GetComponent<CircleLight>();
        runesSpriteRenderer = transform.Find("Runes").GetComponent<SpriteRenderer>();
        runesParticleSystem = runesSpriteRenderer.transform.Find("BouncingUpRays").GetComponent<ParticleSystem>();
        runesTween = runesSpriteRenderer.DOFade(1f, 0.75f).SetLoops(-1, LoopType.Yoyo).Pause();
    }
    private void Update()
    {
        if (InputManager.uniteBtnDown && isCharged && Utils.IsInRange(GameManager.lightSide.transform.position, GameManager.darkSide.transform.position, uniteDistance))
        {
            GameManager.Unite();
            DOVirtual.DelayedCall(timeInEquilibriumForm, GameManager.Separate);
            runesParticleSystem.Stop();
            isCharged = false;
            _etherCount = 0;
            runesSpriteRenderer.DOFade(0f, 0f);
            circleLight.range = 0f;
        }
    }
}
