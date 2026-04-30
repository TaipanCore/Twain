using UnityEngine;

public class SleepingTorchBehaviour : MonoBehaviour, IAbleAggro
{
    private bool _isAggro;
    public bool isAggro
    {
        get => _isAggro;
        set
        {
            if (!_isAggro && value)
            {
                _isAggro = true;
                lightSource.SetActive(true);
                animator.Restart();
            }
        }
    }
    
    private SimpleAnimator animator;
    private GameObject lightSource;
    
    private void Start()
    {
        animator = GetComponent<SimpleAnimator>();
        lightSource = transform.GetChild(0).gameObject;
    }
}
