using System.Collections;
using UnityEngine;

public class EquilibriumBehaviour : MonoBehaviour, IDamageDealer
{
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
    private WaitForSeconds attackDelay;
    private bool canAttack = true;

    [Header("Shoot")]
    [SerializeField] private float shootCooldown;
    private WaitForSeconds shootDelay;
    private bool canShoot = true;
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
        attackDelay = new WaitForSeconds(attackCooldown);
        shootDelay = new WaitForSeconds(shootCooldown);
    }
    private void Update()
    {
        animator.SetFloat("MovSpeed", movement.currentSpeed);
        if (movement.currentSpeed < 0.01f)
        {
            if (InputManager.leftMouseBtnDown && canAttack)
            {
                StartCoroutine(Attack());
            }
            if (InputManager.rightMouseBtnDown && canShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }
    private IEnumerator Attack()
    {
        animator.SetTrigger("PlayAttack");
        canAttack = false;
        yield return attackDelay;
        canAttack = true;
    }
    private IEnumerator Shoot()
    {
        animator.SetTrigger("PlayShoot");
        canShoot = false;
        yield return shootDelay;
        canShoot = true;
    }
    private void SpawnBullet()
    {
        Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
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
