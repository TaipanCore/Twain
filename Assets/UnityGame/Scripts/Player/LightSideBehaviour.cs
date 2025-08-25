using System.Collections;
using UnityEngine;
public class LightSideBehaviour : MonoBehaviour, IDamageReceiver
{
    private enum State
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
    
    [SerializeField] private float invulnerableTime;

    [SerializeField] private LightSideMovement movement;

    [SerializeField] private LightSource circleLight;
    [SerializeField] private float baseCircleLightRange;
    [SerializeField] private float focusedCircleLightRange;

    [SerializeField] private LightSource distantLight;
    [SerializeField] private float focusedDistantLightRange;
    [SerializeField] private float lightRotationSpeed;

    private State state;
    private Transform distantLightTransform;
    
    private void Awake()
    {
        GameManager.LightSide = gameObject;
    }
    private void Start()
    {       
        distantLightTransform = distantLight.gameObject.GetComponent<Transform>();
        SetState(State.Normal);
    }
    private void Update()
    {
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
    private void SetState(State newState)
    {
        if (state != newState)
        {
            state = newState;
            switch (state)
            {
                case State.Normal:
                    SetNormalSettings();
                    break;
                case State.Focused:
                    SetFocusedSettings();
                    break;
            }
        }       
    }
    private void SetNormalSettings()
    {
        circleLight.range = baseCircleLightRange;
        distantLight.gameObject.SetActive(false);
        movement.moveSpeed = movement.baseMovSpeed;
    }
    private void SetFocusedSettings()
    {
        circleLight.range = focusedCircleLightRange;
        distantLight.gameObject.SetActive(true);
        distantLight.range = focusedDistantLightRange;
        distantLightTransform.rotation = CalculateRotationAngle();
        movement.moveSpeed = movement.focusedMovSpeed;
    }
    private void NormalBehaviour()
    {
        if (InputManager.leftMouseBtnDown)
        {
            SetState(State.Focused);
        }
    }
    private void FocusedBehaviour()
    {
        if (InputManager.leftMouseBtnUp)
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

    private void OnTriggerStay2D(Collider2D other)
    {
        IDamageDealer damageDealer = other.GetComponent<IDamageDealer>();
        if (damageDealer != null && !isInvulnerable)
        {
            ReceiveDamage(damageDealer.damage);
        }
    }
    public void ReceiveDamage(float damage)
    {
        hitpoints -= damage;
        StartCoroutine(GiveInvulnerability());
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    private bool isInvulnerable = false;
    private IEnumerator GiveInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }
}
