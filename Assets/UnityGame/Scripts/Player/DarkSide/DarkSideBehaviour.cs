using System.Collections;
using UnityEngine;

public class DarkSideBehaviour : MonoBehaviour
{
    [SerializeField, Min(0)] public float lifetimeInDarkness;
    private WaitForSeconds dieInDarknessDelay;
    
    [Header("Spikes")]
    [SerializeField, Min(0)] private float spikesSpawnCooldown;
    private float spikesCooldownTimer;
    [SerializeField, Min(0)] private int numberOfSpikes;
    [SerializeField, Min(0)] private float spikesLifetime;
    [SerializeField] private GameObject spikesSpawner;
    
    private Transform objectTransform;
    private Transform fireflyTransform;
    private LightSideBehaviour lightSideBehaviour;
    private Coroutine dieCoroutine;
    
    private void Awake()
    {
        GameManager.darkSide = gameObject;
    }

    private void Start()
    {
        objectTransform = GetComponent<Transform>();
        fireflyTransform = GameManager.lightSide.transform.Find("Firefly");
        lightSideBehaviour = GameManager.lightSide.GetComponent<LightSideBehaviour>();
        dieInDarknessDelay = new WaitForSeconds(lifetimeInDarkness);
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
    private void FixedUpdate()
    {
        if (!Utils.IsInRange(fireflyTransform.position, objectTransform.position, lightSideBehaviour.GetCurrentLightRange()))
        {
            dieCoroutine ??= StartCoroutine(DieInDarkness());
        }
        else
        {
            if (dieCoroutine != null)
            {
                StopCoroutine(dieCoroutine);
                dieCoroutine = null;
            }
        }
    }
    
    private void SpawnSpikes()
    {
        GameObject spawner = Instantiate(spikesSpawner, objectTransform.position, Quaternion.identity);
        spawner.GetComponent<SpikesSpawner>().Initialize(numberOfSpikes,  spikesLifetime);
        spikesCooldownTimer = spikesSpawnCooldown;
    }
    private IEnumerator DieInDarkness()
    {
        yield return dieInDarknessDelay;
        Destroy(gameObject);
    }
}
