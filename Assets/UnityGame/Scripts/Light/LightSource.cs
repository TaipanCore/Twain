using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] private bool isAggroTrigger;
    protected float _range;
    public virtual float range
    {
        get { return _range; }
        set
        {
            _range = value;
            transform.localScale = Vector3.one * (_range * 2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAggroTrigger)
        {
            if (collision.TryGetComponent(out IAbleAggro ableAggro))
            {
                ableAggro.isAggro = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarkSideBehaviour darkSideBehaviour))
        {
            if (!darkSideBehaviour.isInLight)
                darkSideBehaviour.isInLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarkSideBehaviour darkSideBehaviour))
        {
            darkSideBehaviour.isInLight = false;
        }
    }
}
