using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSideBehaviour : SidesBehaviour
{
    private void Awake()
    {
        GameManager.DarkSide = gameObject;
    }
}
