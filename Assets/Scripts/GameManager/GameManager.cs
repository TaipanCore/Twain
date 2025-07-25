using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Equilibrium;
    [SerializeField] private GameObject LightSide;
    [SerializeField] private GameObject DarkSide;
    [SerializeField] private Camera MainCamera;

    public GameObject currentCharacter;
    [HideInInspector] public bool isUnited;

    private void Start()
    {
        Unite();
    }
    private void Update()
    {
        CheckInputs();
    }

    private void Unite()
    {
        LightSide.SetActive(false);
        DarkSide.SetActive(false);
        Equilibrium.SetActive(true);
        Equilibrium.transform.position = currentCharacter.transform.position;
        currentCharacter = Equilibrium;
        isUnited = !isUnited;
    }
    private void Separate()
    {
        Equilibrium.SetActive(false);
        LightSide.SetActive(true);
        DarkSide.SetActive(true);
        LightSide.transform.position = currentCharacter.transform.position;
        DarkSide.transform.position = currentCharacter.transform.position;
        currentCharacter = LightSide;
        isUnited = !isUnited;
    }
    private void ChangeSide()
    {
        if (currentCharacter == LightSide)
        {
            currentCharacter = DarkSide;
        }
        else
        {
            currentCharacter = LightSide;
        }
    }
    private void CheckInputs()
    {
        if (isUnited)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Separate();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && LightSide.GetComponent<LightSideBehaviour>().isOnTrigger)
            {
                Unite();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ChangeSide();
            }
        }
    }
}
