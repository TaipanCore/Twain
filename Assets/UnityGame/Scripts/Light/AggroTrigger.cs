using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IAbleAggro ableAggro = collision.GetComponent<IAbleAggro>();
        if (ableAggro != null)
        {
            ableAggro.isAggro = true;
        }
    }
}
