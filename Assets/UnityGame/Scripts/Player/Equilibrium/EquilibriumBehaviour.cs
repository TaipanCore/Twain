using System;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using BulletData = BulletBehaviour.BulletData;

public class EquilibriumBehaviour : MonoBehaviour, ISaveLoadObject
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int PlayAttack = Animator.StringToHash("PlayAttack");
    private static readonly int PlayShoot = Animator.StringToHash("PlayShoot");

    [SerializeField] private float timeInEquilibriumForm;

    [Header("Attack")]
    [SerializeField] private float _damage;
    public float damage
    {
        get => _damage;
        set
        {
            _damage = value;
            hookBehaviour.GetComponent<HookBehaviour>().damage = _damage;
        }
    }
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private HookBehaviour hookBehaviour;

    [Header("Shoot")]
    [SerializeField] private float shootCooldown;
    private float shootCooldownTimer;
    [SerializeField] private float bulletStunTime;
    [SerializeField] private GameObject bulletPrefab;

    [Header("HUD elements")]
    [SerializeField] private Image equilibriumChargeBackground;

    private Animator animator;
    private PlayerMovement movement;
    private Transform spawnPoint;
    
    private GameObject currentBullet;
    private Tween equilibriumFormTween;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
        gameObject.SetActive(false);
    }
    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animator = transform.Find("Appearance").GetComponent<Animator>();
        hookBehaviour = transform.Find("Appearance").Find("Hook").GetComponent<HookBehaviour>();
        hookBehaviour.damage = damage;
        spawnPoint = transform.Find("BulletSpawnPoint");
    }

    private void OnEnable()
    {
        StartEquilibriumFormTimer(timeInEquilibriumForm);
    }
    private void Update()
    {
        animator.SetFloat(MovSpeed, movement.currentSpeed);
        if (gameObject.activeInHierarchy)
        {
            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
                G.HUD.mouseCursor.SetAttackRecharge((attackCooldown - attackCooldownTimer) / attackCooldown);
            }

            if (shootCooldownTimer > 0f)
            {
                shootCooldownTimer -= Time.deltaTime;
                G.HUD.mouseCursor.SetShootRecharge((shootCooldown - shootCooldownTimer) / shootCooldown);
            }
        }
        if (movement.currentSpeed < 0.01f)
        {
            if (G.input.leftMouseBtnDown && attackCooldownTimer <= 0f)
            {
                Attack();
            }
            if (G.input.rightMouseBtnDown && shootCooldownTimer <= 0f)
            {
                Shoot();
            }
        }
    }

    private void StartEquilibriumFormTimer(float time)
    {
        equilibriumFormTween ??= DOVirtual.Float(time / timeInEquilibriumForm, 0f, time, value =>
        {
            equilibriumChargeBackground.fillAmount = value;
        }).OnComplete(() =>
        {
            equilibriumChargeBackground.fillAmount = 1f;
            equilibriumChargeBackground.DOFade(0.5f, 0f);
            equilibriumFormTween = null;
            G.characters.Separate();
        });
    }
    private void Attack()
    {
        animator.SetTrigger(PlayAttack);
        G.HUD.mouseCursor.SetAttackRecharge(0f);
        attackCooldownTimer = attackCooldown;
    }
    private void Shoot()
    {
        animator.SetTrigger(PlayShoot);
        G.HUD.mouseCursor.SetShootRecharge(0f);
        shootCooldownTimer = shootCooldown;
    }
    public void SpawnBullet()
    {
        currentBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        currentBullet.GetComponent<BulletBehaviour>().Initialize(bulletStunTime, OnBulletDestroyed);
    }

    private void OnBulletDestroyed()
    {
        currentBullet = null;
    }
    public void ShakeCameraFromSteps()
    {
        G.mainCamera.transform.DOShakePosition(0.3f, 0.075f);
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            transform.position,
            timeInEquilibriumForm - equilibriumFormTween.Elapsed(false),
            attackCooldownTimer,
            shootCooldownTimer,
            currentBullet ? currentBullet.GetComponent<BulletBehaviour>().PackBulletData() : null
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - position
        transform.position = ((JObject)dataToUnpack.data[0]).ToObject<Vector3>();
        //data[1] - remainingTimeInEquilibriumForm
        if (float.TryParse(dataToUnpack.data[1].ToString(), out var parsedRemainingTimeInEquilibriumForm))
            StartEquilibriumFormTimer(parsedRemainingTimeInEquilibriumForm);
        //data[2] - attackCooldownTimer
        if (float.TryParse(dataToUnpack.data[2].ToString(), out var parsedAttackCooldownTimer))
            attackCooldownTimer = parsedAttackCooldownTimer;
        //data[3] - shootCooldownTimer
        if (float.TryParse(dataToUnpack.data[3].ToString(), out var parsedShootCooldownTimer))
            shootCooldownTimer = parsedShootCooldownTimer;
        //data[4] - bulletData
        if (dataToUnpack.data[4] != null)
        {
            BulletData bulletData = ((JObject)dataToUnpack.data[4]).ToObject<BulletData>();
            currentBullet = Instantiate(bulletPrefab);
            BulletBehaviour bulletBehaviour = currentBullet.GetComponent<BulletBehaviour>();
            bulletBehaviour.Initialize(bulletStunTime, OnBulletDestroyed);
            bulletBehaviour.UnpackBulletData(bulletData);
        }
    }
}
