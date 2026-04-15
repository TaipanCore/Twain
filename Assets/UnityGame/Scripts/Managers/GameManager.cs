using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters info")]
    public static GameObject equilibrium;
    public static GameObject lightSide;
    public static GameObject darkSide;

    public static GameObject currentCharacter;
    public static bool isUnited;
    
    [Header("Inventory")]
    public static List<GameObject> inventory = new List<GameObject>();

    [Header("Player & enemy layer masks")]
    public static LayerMask playerMask;
    public static LayerMask enemyMask;
    
    [Header("Camera")]
    public static GameObject mainCamera;
    
    [Header("Shrine of balance")]
    public static GameObject shrineOfBalance;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        playerMask = LayerMask.GetMask("Player");
        enemyMask = LayerMask.GetMask("Enemy");
        Separate();
        InputManager.canPlayerInput = true;
    }
    private void Update()
    {
        CheckInputs();
    }

    public static void Unite()
    {
        lightSide.SetActive(false);
        darkSide.SetActive(false);
        equilibrium.SetActive(true);
        if (currentCharacter)
            equilibrium.transform.position = lightSide.transform.position;
        currentCharacter = equilibrium;
        isUnited = true;
    }
    public static void Separate()
    {
        equilibrium.SetActive(false);
        lightSide.SetActive(true);
        darkSide.SetActive(true);
        if (currentCharacter)
        {
            lightSide.transform.position = currentCharacter.transform.position;
            darkSide.transform.position = currentCharacter.transform.position;
        }       
        currentCharacter = lightSide;
        isUnited = false;
    }
    private void ChangeSide()
    {
        if (currentCharacter == lightSide)
        {
            currentCharacter = darkSide;
        }
        else
        {
            currentCharacter = lightSide;
        }
    }
    private void CheckInputs()
    {
        if (InputManager.sidesChangeBtnDown && !isUnited)
        {
            ChangeSide();
        }
    }
}
