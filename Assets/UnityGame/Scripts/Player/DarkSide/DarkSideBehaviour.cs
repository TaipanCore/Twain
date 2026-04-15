using UnityEngine;

public class DarkSideBehaviour : MonoBehaviour
{
    [Header("Spikes")]
    [SerializeField, Min(0)] private float spikesSpawnCooldown;
    private float spikesCooldownTimer;
    [SerializeField, Min(0)] private int numberOfSpikes;
    [SerializeField, Min(0)] private float spikesLifetime;
    [SerializeField] private GameObject spikesSpawner;
    [SerializeField] private float eyesSpawnMaxRadius;

    private Transform objectTransform;
    
    private void Awake()
    {
        GameManager.darkSide = gameObject;
    }

    private void Start()
    {
        objectTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (spikesCooldownTimer > 0f && gameObject.activeInHierarchy)
        {
            spikesCooldownTimer -= Time.deltaTime;
        }
        if (InputManager.leftMouseBtnDown && spikesCooldownTimer <= 0f && GameManager.currentCharacter == gameObject)
        {
            SpawnSpikes();
        }
    }

    private void SpawnSpikes()
    {
        GameObject spawner = Instantiate(spikesSpawner, objectTransform.position, Quaternion.identity);
        spawner.GetComponent<SpikesSpawner>().Initialize(numberOfSpikes,  spikesLifetime);
        spikesCooldownTimer = spikesSpawnCooldown;
    }
}
