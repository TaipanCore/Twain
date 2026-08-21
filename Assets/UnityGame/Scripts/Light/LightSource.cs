using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    [SerializeField] private bool isAggroTrigger;
    protected float range;

    private Dictionary<DarknessDeath, bool> charactersInLight = new ();
    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAggroTrigger)
        {
            if (collision.TryGetComponent(out IAbleAggro ableAggro))
            {
                ableAggro.isAggro = true;
            }
        }
        
    }
    
    protected virtual void FixedUpdate()
    {
        DarknessDeath[] keys = charactersInLight.Keys.ToArray();
        foreach (DarknessDeath key in keys)
        {
            if (charactersInLight.TryGetValue(key, out bool value))
            {
                if (!value)
                {
                    key.ExitLight(this);
                    charactersInLight.Remove(key);
                }
                else
                {
                    charactersInLight[key] = false;
                }
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DarknessDeath darknessDeath))
        {
            if (!charactersInLight.ContainsKey(darknessDeath))
            {
                darknessDeath.EnterLight(this);
                charactersInLight.Add(darknessDeath, true);
            }
            else
            {
                charactersInLight[darknessDeath] = true;
            }
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
