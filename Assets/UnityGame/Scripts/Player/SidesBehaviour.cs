using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidesBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isOnTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
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
