using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSideBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isOnTrigger;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isOnTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isOnTrigger = false;
        }
    }
}
