using DG.Tweening;
using UnityEngine;

public class FocusedLight : LightSource
{
    [HideInInspector] public float etherDisappearSpeed;
    
    private Tween expandScaleAnim;
    private void Start()
    {
        expandScaleAnim = transform.DOScale(transform.localScale, 0.8f).From(Vector3.zero).SetAutoKill(false);
    }
    private void OnEnable()
    {
        expandScaleAnim.Restart();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IEtherContainer etherContainer))
        {
            etherContainer.SpawnEtherParticle();
        }
    }
}
