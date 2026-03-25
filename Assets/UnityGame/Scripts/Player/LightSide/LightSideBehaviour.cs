using DG.Tweening;
using UnityEngine;
public class LightSideBehaviour : MonoBehaviour, IDamageReceiver
{
    private static readonly int IsFocused = Animator.StringToHash("IsFocused");

    public enum State
    {
        Normal,
        Focused
    }

    [SerializeField] private float _hitpoints;
    public float hitpoints
    {
        get => _hitpoints;
        set
        {
            _hitpoints = value;
            if (_hitpoints <= 0)
                Die();
        }
    }
    
    [SerializeField] private float _invulnerableTime;
    private float invulnerableTimer;
    public float invulnerableTime
    {
        get => _invulnerableTime;
        set => _invulnerableTime = value;
    }

    [Header("Circle light")]
    [SerializeField] private CircleLight circleLight;
    [SerializeField] private float baseCircleLightRange;
    [SerializeField] private float focusedCircleLightRange;

    [Header("Distant light")]
    [SerializeField] private FocusedLight distantLight;
    [SerializeField] private float focusedDistantLightRange;
    [SerializeField] private float lightRotationSpeed;

    private State state;
    private Transform distantLightTransform;
    private LightSideMovement movement;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        GameManager.lightSide = gameObject;
    }
    private void Start()
    {
        animator =  transform.Find("Appearance").GetComponent<Animator>();
        spriteRenderer = transform.Find("Appearance").GetComponent<SpriteRenderer>();
        movement = GetComponent<LightSideMovement>();
        distantLightTransform = distantLight.gameObject.GetComponent<Transform>();
        SetState(State.Normal);
    }
    private void Update()
    {
        if (invulnerableTimer > 0f)
            invulnerableTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Normal:
                NormalBehaviour();
                break;
            case State.Focused:
                FocusedBehaviour();
                break;
        }
    }
    public void SetState(State newState)
    {
        state = newState;
        switch (state)
        {
            case State.Normal:
                SetNormalState();
                break;
            case State.Focused:
                SetFocusedState();
                break;
        }       
    }
    public State GetState()
    {
        return state;
    }
    private void SetNormalState()
    {
        circleLight.range = baseCircleLightRange;
        distantLight.gameObject.SetActive(false);
        movement.moveSpeed = movement.baseMoveSpeed;
        animator.SetBool(IsFocused, false);
    }
    private void SetFocusedState()
    {
        circleLight.range = focusedCircleLightRange;
        distantLight.range = focusedDistantLightRange;
        distantLightTransform.rotation = CalculateRotationAngle();
        distantLight.gameObject.SetActive(true);
        movement.moveSpeed = movement.focusedMoveSpeed;
        animator.SetBool(IsFocused, true);
    }
    private void NormalBehaviour()
    {
        if (InputManager.leftMouseBtn && GameManager.currentCharacter == gameObject)
        {
            SetState(State.Focused);
        }
    }
    private void FocusedBehaviour()
    {
        if (!(InputManager.leftMouseBtn && GameManager.currentCharacter == gameObject))
        {
            SetState(State.Normal);
        }
        distantLightTransform.rotation = Quaternion.Lerp(distantLightTransform.rotation, CalculateRotationAngle(), Time.deltaTime * lightRotationSpeed);
    }
    private Quaternion CalculateRotationAngle()
    {
        Vector3 vectorToTarget = MouseTracker.mousePosition - distantLightTransform.position;
        return Quaternion.Euler(0, 0, Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);
    }

    public float GetCurrentLightRange()
    {
        return state == State.Normal ? baseCircleLightRange : focusedCircleLightRange;
    }
    public void ReceiveDamage(float damage)
    {
        if (invulnerableTimer <= 0f)
        {
            hitpoints -= damage;
            GiveInvulnerability();
        }
    }
    public void GiveInvulnerability()
    {
        Tween blinkingTween = spriteRenderer.DOFade(0.4f, 0.15f).SetLoops(-1, LoopType.Yoyo);
        DOVirtual.DelayedCall(invulnerableTime, () =>
        {
            blinkingTween.Kill();
            spriteRenderer.DOFade(1f, 0f);
        });
        invulnerableTimer = invulnerableTime;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
