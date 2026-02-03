using System.Collections;
using UnityEngine;
public class LightSideBehaviour : MonoBehaviour, IDamageReceiver
{
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
    private WaitForSeconds invulnerableDelay;
    public float invulnerableTime
    {
        get => _invulnerableTime;
        set => _invulnerableTime = value;
    }
    public bool isInvulnerable { get; set; } = false;

    [Header("Circle light")]
    [SerializeField] private LightSource circleLight;
    [SerializeField] private float baseCircleLightRange;
    [SerializeField] private float focusedCircleLightRange;

    [Header("Distant light")]
    [SerializeField] private LightSource distantLight;
    [SerializeField] private float focusedDistantLightRange;
    [SerializeField] private float lightRotationSpeed;

    private State state;
    private Transform distantLightTransform;
    private LightSideMovement movement;
    private Animator animator;

    private void Awake()
    {
        GameManager.LightSide = gameObject;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<LightSideMovement>();
        distantLightTransform = distantLight.gameObject.GetComponent<Transform>();
        invulnerableDelay = new WaitForSeconds(invulnerableTime);
        SetState(State.Normal);
    }
    private void Update()
    {
        animator.SetFloat("MovSpeed", movement.currentSpeed);
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
        if (state != newState)
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
        animator.SetBool("IsFocused", false);
    }
    private void SetFocusedState()
    {
        circleLight.range = focusedCircleLightRange;
        distantLight.gameObject.SetActive(true);
        distantLight.range = focusedDistantLightRange;
        distantLightTransform.rotation = CalculateRotationAngle();
        movement.moveSpeed = movement.focusedMoveSpeed;
        animator.SetBool("IsFocused", true);
    }
    private void NormalBehaviour()
    {
        if (InputManager.leftMouseBtnDown && GameManager.currentCharacter == gameObject)
        {
            SetState(State.Focused);
        }
    }
    private void FocusedBehaviour()
    {
        if (InputManager.leftMouseBtnUp && GameManager.currentCharacter == gameObject)
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

    public void ReceiveDamage(float damage)
    {
        if (!isInvulnerable)
        {
            hitpoints -= damage;
            StartCoroutine(GiveInvulnerability());
        }
    }
    public IEnumerator GiveInvulnerability()
    {
        isInvulnerable = true;
        yield return invulnerableDelay;
        isInvulnerable = false;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
