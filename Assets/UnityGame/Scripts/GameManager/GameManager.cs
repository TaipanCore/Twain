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

    public static GameObject currentCharacter;
    [HideInInspector] public static bool isUnited;

    private void Start()
    {
        Application.targetFrameRate = 60;
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
        if (currentCharacter != null)
            Equilibrium.transform.position = currentCharacter.transform.position;
        currentCharacter = Equilibrium;
        isUnited = !isUnited;
    }
    private void Separate()
    {
        Equilibrium.SetActive(false);
        Physics2D.IgnoreCollision(LightSide.GetComponent<BoxCollider2D>(), DarkSide.GetComponent<BoxCollider2D>(), true);
        LightSide.SetActive(true);
        DarkSide.SetActive(true);
        if (currentCharacter != null)
        {
            LightSide.transform.position = currentCharacter.transform.position;
            DarkSide.transform.position = currentCharacter.transform.position;
        }       
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
            if (InputManager.uniteAndSeparateBtnDown)
            {
                Separate();
            }
        }
        else
        {
            if (InputManager.uniteAndSeparateBtnDown && (LightSide.GetComponent<LightSideBehaviour>().isOnPlayerTrigger || DarkSide.GetComponent<DarkSideBehaviour>().isOnPlayerTrigger))
            {
                Unite();
            }
            if (InputManager.sidesChangeBtnDown)
            {
                ChangeSide();
            }
        }
    }
}
