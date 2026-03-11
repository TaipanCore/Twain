using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode uniteAndSeparateBtn;
    [SerializeField] private KeyCode sidesChangeBtn;

    [HideInInspector] public static bool uniteBtnDown;
    [HideInInspector] public static bool sidesChangeBtnDown;
    [HideInInspector] public static Vector2 movement;
    [HideInInspector] public static bool leftMouseBtn;
    [HideInInspector] public static bool leftMouseBtnDown;
    [HideInInspector] public static bool leftMouseBtnUp;
    [HideInInspector] public static bool rightMouseBtn;
    [HideInInspector] public static bool rightMouseBtnDown;
    [HideInInspector] public static bool rightMouseBtnUp;

    private void Update()
    {
        uniteBtnDown = Input.GetKeyDown(uniteAndSeparateBtn);
        sidesChangeBtnDown = Input.GetKeyDown(sidesChangeBtn);
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        leftMouseBtn = Input.GetMouseButton(0);
        leftMouseBtnDown = Input.GetMouseButtonDown(0);
        leftMouseBtnUp = Input.GetMouseButtonUp(0);
        rightMouseBtn = Input.GetMouseButton(1);
        rightMouseBtnDown = Input.GetMouseButtonDown(1);
        rightMouseBtnUp = Input.GetMouseButtonUp(1);
    }
}
