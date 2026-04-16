using DG.Tweening;
using UnityEngine;

public class FocusedLight : LightSource
{
    private Tween expandScaleAnim;
    public override float range
    {
        get { return _range; }
        set
        {
            _range = value;
            expandScaleAnim.Restart();
        }
    }
    private void Start()
    {
        expandScaleAnim = transform.DOScale(Vector3.one * _range, 0.8f).From(Vector3.zero).SetAutoKill(false);
    }
    private void OnEnable()
    {
        expandScaleAnim.Restart();
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IEtherContainer etherContainer))
        {
            etherContainer.SpawnEtherParticle();
        }
    }
}
