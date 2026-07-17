using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DarknessDeath : MonoBehaviour
{
    [SerializeField, Min(0)] public float lifetimeInDarkness;
    [SerializeField] private GameObject redEyesPrefab;
    [SerializeField] private float eyesSpawnMaxRadius;
    
    public HashSet<LightSource> lightSources { get; private set; } = new ();
    
    private Transform objectTransform;
    private Coroutine dieCoroutine;
    private GameObject redEyesContainer;
    private HashSet<GameObject> redEyesSet = new ();
    private float invulnerabilityTimer;
    private float coroutineElapsedTime;

    private void Start()
    {
        redEyesContainer = InitializeContainer();
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
            invulnerabilityTimer -= Time.deltaTime;
    }
    private void LateUpdate()
    {
        redEyesContainer.transform.rotation = Quaternion.identity;
    }

    public GameObject InitializeContainer()
    {
        objectTransform = GetComponent<Transform>();
        if (transform.Find("RedEyesContainer"))
            return transform.Find("RedEyesContainer").gameObject;
        GameObject container = new GameObject("RedEyesContainer");
        return Instantiate(container, objectTransform.position, Quaternion.identity, transform);
    }

    public void EnterLight(LightSource source)
    {
        lightSources.Add(source);
        if (dieCoroutine != null)
        {
            StopCoroutine(dieCoroutine);
            dieCoroutine = null;
            DestroyAllEyes();
        }
    }
    public void ExitLight(LightSource source)
    {
        if (!gameObject.activeInHierarchy)
            return;
        lightSources.Remove(source);
        if (lightSources.Count == 0)
        {
            dieCoroutine ??= StartCoroutine(DieInDarkness());
        }
    }

    public void GiveDarknessInvulnerability(float time)
    {
        invulnerabilityTimer = time;
    }
    private void DestroyAllEyes()
    {
        foreach (GameObject eye in redEyesSet)
        {
            eye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetEase(Ease.InCubic).OnComplete(() => Destroy(eye));
        }
        redEyesSet.Clear();
    }
    private IEnumerator DieInDarkness(float elapsedTime = 0f)
    {
        coroutineElapsedTime = elapsedTime;
        while (coroutineElapsedTime < lifetimeInDarkness)
        {
            float currentDelay = Mathf.Lerp(0.5f, 0.1f, coroutineElapsedTime / lifetimeInDarkness);
            yield return new WaitForSeconds(currentDelay);
            Vector3 insideCirclePosition = Random.insideUnitCircle;
            redEyesSet.Add(Instantiate(redEyesPrefab, objectTransform.position + insideCirclePosition.normalized + insideCirclePosition * eyesSpawnMaxRadius, Quaternion.identity, redEyesContainer.transform));
            coroutineElapsedTime += currentDelay;
        }
        yield return new WaitWhile(() => invulnerabilityTimer > 0);
        G.characters.GameOver();
    }

    public DarknessDeathData PackDarknessDeathData()
    {
        List<Vector3> redEyesPositions = new ();
        foreach (GameObject eye in redEyesSet)
            redEyesPositions.Add(eye.transform.position);
        return new DarknessDeathData(dieCoroutine != null, coroutineElapsedTime, redEyesPositions.ToArray());
    }

    public void UnpackDarknessDeathData(DarknessDeathData data)
    {
        if (data.isDieCoroutineActive)
        {
            dieCoroutine ??= StartCoroutine(DieInDarkness(data.elapsedTime));
            foreach (Vector3 eyePosition in data.redEyesPositions)
                redEyesSet.Add(Instantiate(redEyesPrefab, eyePosition, Quaternion.identity, redEyesContainer.transform));
        }
    }

    [Serializable]
    public class DarknessDeathData
    {
        public DarknessDeathData(bool isDieCoroutineActive, float elapsedTime, Vector3[] redEyesPositions)
        {
            this.isDieCoroutineActive = isDieCoroutineActive;
            this.elapsedTime = elapsedTime;
            this.redEyesPositions = redEyesPositions;
        }

        public bool isDieCoroutineActive;
        public float elapsedTime;
        public Vector3[] redEyesPositions;
    }
}
