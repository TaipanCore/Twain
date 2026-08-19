using System;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityGame.Scripts.Interfaces;
using DarknessDeathData = DarknessDeath.DarknessDeathData;

public class LightSideBehaviour : MonoBehaviour, IInvulnerableDamageReceiver, ISaveLoadObject
{
    private static readonly int IsFocused = Animator.StringToHash("IsFocused");

    public enum State
    {
        Normal,
        Focused,
        WithoutFirefly
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
    private Transform fireflyTransform;
    private Vector3 fireflyLocalPoint;
    private TrailRenderer fireflyTrail;
    private DarknessDeath darknessDeath;
    private float maxHitpoints;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
        animator =  transform.Find("Appearance").GetComponent<Animator>();
        spriteRenderer = transform.Find("Appearance").GetComponent<SpriteRenderer>();
        movement = GetComponent<LightSideMovement>();
        distantLightTransform = distantLight.gameObject.GetComponent<Transform>();
        fireflyTransform = transform.Find("Firefly");
        fireflyLocalPoint = fireflyTransform.localPosition;
        fireflyTrail = fireflyTransform.GetComponent<TrailRenderer>();
        darknessDeath = GetComponent<DarknessDeath>();
        maxHitpoints = hitpoints;
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
            case State.WithoutFirefly:
                WithoutFireflyBehaviour();
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
            case State.WithoutFirefly:
                SetWithoutFireflyState();
                break;
        }       
    }

    public State GetState()
    {
        return state;
    }
    private void SetNormalState()
    {
        circleLight.SetRange(baseCircleLightRange, 0.25f);
        distantLight.gameObject.SetActive(false);
        movement.moveSpeed = movement.baseMoveSpeed;
        animator.SetBool(IsFocused, false);
        darknessDeath.EnterLight(circleLight);
    }
    private void SetFocusedState()
    {
        circleLight.SetRange(focusedCircleLightRange, 0.25f);
        distantLight.SetRange(focusedDistantLightRange);
        distantLightTransform.rotation = CalculateRotationAngle();
        distantLight.gameObject.SetActive(true);
        movement.moveSpeed = movement.focusedMoveSpeed;
        animator.SetBool(IsFocused, true);
    }
    private void SetWithoutFireflyState()
    {
        SetNormalState();
    }
    private void NormalBehaviour()
    {
        if (G.input.leftMouseBtn && G.characters.currentCharacter == gameObject)
        {
            SetState(State.Focused);
        }
    }
    private void FocusedBehaviour()
    {
        if (!(G.input.leftMouseBtn && G.characters.currentCharacter == gameObject))
        {
            SetState(State.Normal);
        }
        distantLightTransform.rotation = Quaternion.Lerp(distantLightTransform.rotation, CalculateRotationAngle(), Time.deltaTime * lightRotationSpeed);
    }

    private void WithoutFireflyBehaviour()
    {
        
    }
    private Quaternion CalculateRotationAngle()
    {
        Vector3 vectorToTarget = G.mouseTracker.mousePosition - distantLightTransform.position;
        return Quaternion.Euler(0, 0, Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg);
    }

    public Transform TakeFirefly(Transform newParent, float duration = 0.25f)
    {
        fireflyTransform.SetParent(newParent);
        SetState(State.WithoutFirefly);
        fireflyTrail.emitting = true;
        if (duration != 0f)
            fireflyTransform.DOMove(newParent.position, duration);
        else
            fireflyTransform.position = newParent.position;
        return fireflyTransform;
    }

    public void ReturnFirefly()
    {
        fireflyTransform.SetParent(transform);
        fireflyTransform.DOLocalMove(fireflyLocalPoint, 0.25f).OnComplete(() =>
        {
            SetState(State.Normal);
            fireflyTrail.emitting = false;
        });
    }
    public float GetCurrentLightRange()
    {
        return state == State.Focused ? focusedCircleLightRange : baseCircleLightRange;
    }
    public void ReceiveDamage(float damage)
    {
        if (invulnerableTimer <= 0f)
        {
            hitpoints -= damage;
            GetComponentInChildren<LightSideSounds>().PlayDamagedSound();
            G.HUD.healthBar.SetValue(hitpoints);
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
        }, false);
        invulnerableTimer = invulnerableTime;
    }

    public void RestoreHealth()
    {
        hitpoints = maxHitpoints;
        G.HUD.healthBar.SetValue(hitpoints);
    }
    public void Die()
    {
        G.characters.GameOver();
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            transform.position,
            state,
            hitpoints,
            invulnerableTimer,
            GetComponent<DarknessDeath>().PackDarknessDeathData()
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - position
        transform.position = ((JObject)dataToUnpack.data[0]).ToObject<Vector3>();
        //data[1] - state
        if (Enum.TryParse(dataToUnpack.data[1].ToString(), out State parsedState))
            SetState(parsedState);
        //data[2] - hitpoints
        if (float.TryParse(dataToUnpack.data[2].ToString(), out var parsedHitpoints))
            hitpoints = parsedHitpoints;
        //data[3] - invulnerableTimer
        if (float.TryParse(dataToUnpack.data[3].ToString(), out var parsedInvulnerableTimer))
            invulnerableTimer = parsedInvulnerableTimer;
        //data[4] - darknessDeathData
        DarknessDeathData serializedDarknessDeathData = ((JObject)dataToUnpack.data[4]).ToObject<DarknessDeathData>();
        GetComponent<DarknessDeath>().UnpackDarknessDeathData(serializedDarknessDeathData);
    }
}
