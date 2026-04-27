using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] private bool isAggroTrigger;
    protected float range;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAggroTrigger)
        {
            if (collision.TryGetComponent(out IAbleAggro ableAggro))
            {
                ableAggro.isAggro = true;
            }
        }
        if (collision.TryGetComponent(out DarknessDeath darknessDeath))
        {
            darknessDeath.EnterLight(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarknessDeath darknessDeath))
        {
            darknessDeath.ExitLight(this);
        }
    }
    
    public virtual void SetRange(float newRange)
    {
        range = newRange;
        transform.localScale = Vector3.one * (range * 2f);
    }
    public virtual float GetRange()
    {
        return range;
    }
}
