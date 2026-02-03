using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters info")]
    public static GameObject Equilibrium;
    public static GameObject LightSide;
    public static GameObject DarkSide;

    public static GameObject currentCharacter;
    public static bool isUnited;

    public float uniteDistance;

    [Header("Player & enemy layer masks")]
    public static LayerMask playerMask;
    public static LayerMask enemyMask;
    private void Start()
    {
        Application.targetFrameRate = 60;
        playerMask = LayerMask.GetMask("Player");
        enemyMask = LayerMask.GetMask("Enemy");
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
            if (InputManager.uniteAndSeparateBtnDown && Utils.IsInRange(LightSide.transform.position, DarkSide.transform.position, uniteDistance))
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
