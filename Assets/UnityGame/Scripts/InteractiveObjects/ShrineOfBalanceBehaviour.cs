using DG.Tweening;
using UnityEngine;

public class ShrineOfBalanceBehaviour : MonoBehaviour
{
    [SerializeField] private float uniteDistance;
    [SerializeField] private float timeInEquilibriumForm;
    [SerializeField] private int maxEtherCount;
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
                    runesTween.Rewind();
                    runesSpriteRenderer.DOFade(1f, 0f);
                    runesParticleSystem.Play();
                    isCharged = true;
                }
                else
                {
                   runesIntensity = (float)_etherCount / maxEtherCount;
                   runesTween.timeScale = runesBlinkingIntensity.Evaluate(runesIntensity);
                   if (!runesTween.IsPlaying())
                   {
                       runesTween.Play();
                   } 
                }
            }
            Debug.Log($"{_etherCount}/{maxEtherCount}");
        }
    }
    
    private Transform playerTransform;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer lightSideSpriteRenderer;
    private SpriteRenderer darkSideSpriteRenderer;
    private SpriteRenderer equilibriumSpriteRenderer;
    
    private SpriteRenderer spriteRenderer;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightSideSpriteRenderer = GameManager.lightSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        darkSideSpriteRenderer = GameManager.darkSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        equilibriumSpriteRenderer = GameManager.equilibrium.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        runesSpriteRenderer = transform.Find("Runes").GetComponent<SpriteRenderer>();
        runesParticleSystem = runesSpriteRenderer.transform.Find("BouncingUpRays").GetComponent<ParticleSystem>();
        runesTween = runesSpriteRenderer.DOFade(1f, 0.75f).SetLoops(-1, LoopType.Yoyo).Pause();
    }
    private void Update()
    {
        playerTransform = GameManager.currentCharacter.transform;
        FindPlayerSpriteRenderer();
        CheckSpriteOrderInLayer();
        if (InputManager.uniteBtnDown && isCharged && Utils.IsInRange(GameManager.lightSide.transform.position, GameManager.darkSide.transform.position, uniteDistance))
        {
            GameManager.Unite();
            DOVirtual.DelayedCall(timeInEquilibriumForm, GameManager.Separate);
            runesParticleSystem.Stop();
            isCharged = false;
            _etherCount = 0;
            runesSpriteRenderer.DOFade(0f, 0f);
        }
    }

    private void FindPlayerSpriteRenderer()
    {
        if (GameManager.currentCharacter == GameManager.lightSide)
            playerSpriteRenderer = lightSideSpriteRenderer;
        else if (GameManager.currentCharacter == GameManager.darkSide)
            playerSpriteRenderer = darkSideSpriteRenderer;
        else if (GameManager.currentCharacter == GameManager.equilibrium)
            playerSpriteRenderer = equilibriumSpriteRenderer;
    }
    private void CheckSpriteOrderInLayer()
    {
        if (playerTransform.position.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
        }
        else
        {
            spriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
        }
    }
}
