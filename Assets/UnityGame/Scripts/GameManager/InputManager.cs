using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KeyCode uniteAndSeparateBtn;
    [SerializeField] private KeyCode sidesChangeBtn;

    [HideInInspector] public static bool uniteAndSeparateBtnPressed;
    [HideInInspector] public static bool sidesChangeBtnPressed;
    [HideInInspector] public static Vector2 movement;

    private void Update()
    {
        uniteAndSeparateBtnPressed = Input.GetKeyDown(uniteAndSeparateBtn);
        sidesChangeBtnPressed = Input.GetKeyDown(sidesChangeBtn);
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
