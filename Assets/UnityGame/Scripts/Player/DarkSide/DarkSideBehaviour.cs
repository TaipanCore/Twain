using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DarkSideBehaviour : MonoBehaviour
{
    [SerializeField, Min(0)] public float lifetimeInDarkness;
    
    [Header("Spikes")]
    [SerializeField, Min(0)] private float spikesSpawnCooldown;
    private float spikesCooldownTimer;
    [SerializeField, Min(0)] private int numberOfSpikes;
    [SerializeField, Min(0)] private float spikesLifetime;
    [SerializeField] private GameObject spikesSpawner;
    [SerializeField] private GameObject redEyesPrefab;
    [SerializeField] private float eyesSpawnMaxRadius;
    
    private Transform objectTransform;
    private Transform fireflyTransform;
    private LightSideBehaviour lightSideBehaviour;
    private Coroutine dieCoroutine;
    private GameObject redEyesContainer;
    private HashSet<GameObject> redEyesSet = new ();
    
    private void Awake()
    {
        GameManager.darkSide = gameObject;
    }

    private void Start()
    {
        objectTransform = GetComponent<Transform>();
        fireflyTransform = GameManager.lightSide.transform.Find("Firefly");
        lightSideBehaviour = GameManager.lightSide.GetComponent<LightSideBehaviour>();
        redEyesContainer = transform.Find("RedEyesContainer").gameObject;
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

    private void LateUpdate()
    {
        redEyesContainer.transform.rotation = Quaternion.identity;
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
                DestroyAllEyes();
            }
        }
    }
    private void DestroyAllEyes()
    {
        foreach (GameObject eye in redEyesSet)
        {
            eye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetEase(Ease.InCubic).OnComplete(() => Destroy(eye));
        }
        redEyesSet.Clear();
    }

    private void SpawnSpikes()
    {
        GameObject spawner = Instantiate(spikesSpawner, objectTransform.position, Quaternion.identity);
        spawner.GetComponent<SpikesSpawner>().Initialize(numberOfSpikes,  spikesLifetime);
        spikesCooldownTimer = spikesSpawnCooldown;
    }
    private IEnumerator DieInDarkness()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lifetimeInDarkness)
        {
            Vector3 insideCirclePosition = Random.insideUnitCircle;
            redEyesSet.Add(Instantiate(redEyesPrefab, objectTransform.position + insideCirclePosition.normalized + insideCirclePosition * eyesSpawnMaxRadius, Quaternion.identity, redEyesContainer.transform));
            float currentDelay = Mathf.Lerp(0.5f, 0.1f, elapsedTime / lifetimeInDarkness);
            yield return new WaitForSeconds(currentDelay);
            elapsedTime += currentDelay;
        }
        Destroy(gameObject);
    }
}
