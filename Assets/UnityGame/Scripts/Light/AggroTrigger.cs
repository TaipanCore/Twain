using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IAbleAggro ableAggro))
        {
            ableAggro.isAggro = true;
        }
    }
}
