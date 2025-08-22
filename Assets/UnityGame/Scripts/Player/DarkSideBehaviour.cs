using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSideBehaviour : MonoBehaviour
{
    private void Awake()
    {
        GameManager.DarkSide = gameObject;
    }
}
