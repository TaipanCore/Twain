using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyBehaviour : MonoBehaviour
{
    private float _range;
    [HideInInspector] public float range
    {
        get { return _range; }
        set
        {
            _range = value;
            transform.localScale = Vector3.one * _range;
        }
    }
}
