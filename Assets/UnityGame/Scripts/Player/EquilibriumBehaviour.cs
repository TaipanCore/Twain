using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilibriumBehaviour : MonoBehaviour
{
    [SerializeField] private MouseTracker mouseTracker;

    private void Awake()
    {
        GameManager.Equilibrium = gameObject;
    }
}
