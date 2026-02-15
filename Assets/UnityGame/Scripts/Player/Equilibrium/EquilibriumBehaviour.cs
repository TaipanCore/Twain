using System.Collections;
using UnityEngine;

public class EquilibriumBehaviour : MonoBehaviour, IDamageDealer
{
    private static readonly int MovSpeed = Animator.StringToHash("MovSpeed");
    private static readonly int PlayAttack = Animator.StringToHash("PlayAttack");
    private static readonly int PlayShoot = Animator.StringToHash("PlayShoot");

    [Header("Attack")]
    [SerializeField] private float _damage;
    public float damage
    {
        get => _damage;
        set
        {
            _damage = value;
        }
    }
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;

    [Header("Shoot")]
    [SerializeField] private float shootCooldown;
    private float shootCooldownTimer;
    [SerializeField] private float bulletStunTime;
    [SerializeField] private GameObject bulletPrefab;

    private Animator animator;
    private PlayerMovement movement;
    private Transform spawnPoint;
    private void Awake()
    {
        GameManager.Equilibrium = gameObject;
    }
    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        spawnPoint = transform.Find("BulletSpawnPoint");
    }
    private void Update()
    {
        animator.SetFloat(MovSpeed, movement.currentSpeed);
        if (gameObject.activeInHierarchy)
        {
            if (attackCooldownTimer > 0f)
                attackCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer > 0f)
                shootCooldownTimer -= Time.deltaTime;
        }
        if (movement.currentSpeed < 0.01f)
        {
            if (InputManager.leftMouseBtnDown && attackCooldownTimer <= 0f)
            {
                Attack();
            }
            if (InputManager.rightMouseBtnDown && shootCooldownTimer <= 0f)
            {
                Shoot();
            }
        }
    }
    private void Attack()
    {
        animator.SetTrigger(PlayAttack);
        attackCooldownTimer = attackCooldown;
    }
    private void Shoot()
    {
        animator.SetTrigger(PlayShoot);
        shootCooldownTimer = shootCooldown;
    }
    private void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        bullet.GetComponent<BulletBehaviour>().Initialize(bulletStunTime);
    }
    public void DealDamage(float damage, IDamageReceiver target)
    {
        target.ReceiveDamage(damage);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageReceiver damageReceiver) && other.gameObject.IsInLayerMask(GameManager.enemyMask))
        {
            DealDamage(damage, damageReceiver);
        }
    }
}
