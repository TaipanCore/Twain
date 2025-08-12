using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidesBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isOnPlayerTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isOnPlayerTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isOnPlayerTrigger = false;
        }
    }
}
