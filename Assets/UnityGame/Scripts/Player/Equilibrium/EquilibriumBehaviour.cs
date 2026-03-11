using UnityEngine;

public class EquilibriumBehaviour : MonoBehaviour
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

    private Animator animator;
    private PlayerMovement movement;
    private Transform spawnPoint;
    private CameraTracking mainCameraTracking;
    private void Awake()
    {
        GameManager.equilibrium = gameObject;
    }
    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animator = transform.Find("Appearance").GetComponent<Animator>();
        mainCameraTracking = GameManager.mainCamera.GetComponent<CameraTracking>();
        hookBehaviour = transform.Find("Appearance").Find("Hook").GetComponent<HookBehaviour>();
        hookBehaviour.damage = damage;
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
    public void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        bullet.GetComponent<BulletBehaviour>().Initialize(bulletStunTime);
    }
    public void ShakeCameraFromSteps()
    {
        mainCameraTracking.ShakeCamera(0.3f, 0.075f);
    }
}
