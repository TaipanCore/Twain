using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KeyCode uniteAndSeparateBtn;
    [SerializeField] private KeyCode sidesChangeBtn;

    [HideInInspector] public static bool uniteAndSeparateBtnDown;
    [HideInInspector] public static bool sidesChangeBtnDown;
    [HideInInspector] public static Vector2 movement;
    [HideInInspector] public static bool leftMouseBtnDown;
    [HideInInspector] public static bool leftMouseBtnUp;

    private void Update()
    {
        uniteAndSeparateBtnDown = Input.GetKeyDown(uniteAndSeparateBtn);
        sidesChangeBtnDown = Input.GetKeyDown(sidesChangeBtn);
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        leftMouseBtnDown = Input.GetMouseButtonDown(0);
        leftMouseBtnUp = Input.GetMouseButtonUp(0);
    }
}
