using DG.Tweening;
using UnityEngine;

public class GlobalServices : MonoBehaviour
{
    private static GlobalServices instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        DOTween.SetTweensCapacity(500, 125);
    }
    
}
