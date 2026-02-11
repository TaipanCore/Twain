using System.Collections;
using UnityEngine;

public class DarkSideBehaviour : MonoBehaviour
{
    [SerializeField, Min(0)] public float lifetimeInDarkness;
    private WaitForSeconds dieInDarknessDelay;
    
    private Transform objectTransform;
    private Transform fireflyTransform;
    private LightSideBehaviour lightSideBehaviour;
    private Coroutine dieCoroutine;
    
    private void Awake()
    {
        GameManager.DarkSide = gameObject;
    }

    private void Start()
    {
        objectTransform = GetComponent<Transform>();
        fireflyTransform = GameManager.LightSide.transform.Find("Firefly");
        lightSideBehaviour = GameManager.LightSide.GetComponent<LightSideBehaviour>();
        dieInDarknessDelay = new WaitForSeconds(lifetimeInDarkness);
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
    private IEnumerator DieInDarkness()
    {
        yield return dieInDarknessDelay;
        Destroy(gameObject);
    }
}
