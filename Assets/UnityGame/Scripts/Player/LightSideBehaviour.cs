using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSideBehaviour : SidesBehaviour
{
    [SerializeField] private FireflyBehaviour firefly;
    [SerializeField] private float fireflyRange;
    [SerializeField] private float fireflyFocusedRange;

    private void Start()
    {
        firefly.range = fireflyRange;
    }
}
