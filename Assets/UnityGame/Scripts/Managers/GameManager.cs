using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Characters info")]
    public static GameObject equilibrium;
    public static GameObject lightSide;
    public static GameObject darkSide;

    public static GameObject currentCharacter;
    public static bool isUnited;
    public static event Action<GameObject> OnCharacterChange;

    [Header("HUD")]
    public static HUD HUD;

    [Header("Player & enemy layer masks")]
    public static LayerMask playerMask;
    public static LayerMask enemyMask;
    
    [Header("Cameras")]
    public static GameObject mainCamera;
    public static GameObject mapCamera;
    
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
        OnCharacterChange?.Invoke(currentCharacter);
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
        OnCharacterChange?.Invoke(currentCharacter);
    }
    private void CheckInputs()
    {
        if (InputManager.sidesChangeBtnDown && !isUnited)
        {
            ChangeSide();
        }
    }
}
