using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DarknessDeath : MonoBehaviour
{
    [SerializeField, Min(0)] public float lifetimeInDarkness;
    [SerializeField] private GameObject redEyesPrefab;
    [SerializeField] private float eyesSpawnMaxRadius;
    
    private bool _isInLight;
    public bool isInLight
    {
        get => _isInLight;
        set
        {
            _isInLight = value;
            if (!gameObject.activeInHierarchy)
                return;
            if (!_isInLight)
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
    }
    private Transform objectTransform;
    private Coroutine dieCoroutine;
    private GameObject redEyesContainer;
    private HashSet<GameObject> redEyesSet = new ();

    private void Start()
    {
        objectTransform = GetComponent<Transform>();
        if (transform.Find("RedEyesContainer"))
        {
            redEyesContainer = transform.Find("RedEyesContainer").gameObject;
        }
        else
        {
            GameObject container = new GameObject("RedEyesContainer");
            redEyesContainer = Instantiate(container, objectTransform.position, Quaternion.identity, transform);
        }
    }
    private void LateUpdate()
    {
        redEyesContainer.transform.rotation = Quaternion.identity;
    }
    
    private void DestroyAllEyes()
    {
        foreach (GameObject eye in redEyesSet)
        {
            eye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).SetEase(Ease.InCubic).OnComplete(() => Destroy(eye));
        }
        redEyesSet.Clear();
    }
    private IEnumerator DieInDarkness()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lifetimeInDarkness)
        {
            float currentDelay = Mathf.Lerp(0.5f, 0.1f, elapsedTime / lifetimeInDarkness);
            yield return new WaitForSeconds(currentDelay);
            if (GameManager.currentCharacter == gameObject)
            {
                Vector3 insideCirclePosition = Random.insideUnitCircle;
                redEyesSet.Add(Instantiate(redEyesPrefab, objectTransform.position + insideCirclePosition.normalized + insideCirclePosition * eyesSpawnMaxRadius, Quaternion.identity, redEyesContainer.transform));
            }
            elapsedTime += currentDelay;
        }
        Destroy(gameObject);
    }
}
