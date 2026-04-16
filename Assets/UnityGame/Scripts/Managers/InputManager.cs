using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode uniteBtn;
    [SerializeField] private KeyCode sidesChangeBtn;
    [SerializeField] private KeyCode interactiveBtn;

    [HideInInspector] public static bool canPlayerInput;
        
    [HideInInspector] public static bool uniteBtnDown;
    [HideInInspector] public static bool sidesChangeBtnDown;
    [HideInInspector] public static bool interactiveBtnDown;
    [HideInInspector] public static Vector2 movement;
    [HideInInspector] public static bool leftMouseBtn;
    [HideInInspector] public static bool leftMouseBtnDown;
    [HideInInspector] public static bool leftMouseBtnUp;
    [HideInInspector] public static bool rightMouseBtn;
    [HideInInspector] public static bool rightMouseBtnDown;
    [HideInInspector] public static bool rightMouseBtnUp;

    private void Update()
    {
        uniteBtnDown = canPlayerInput && Input.GetKeyDown(uniteBtn);
        sidesChangeBtnDown = canPlayerInput && Input.GetKeyDown(sidesChangeBtn);
        interactiveBtnDown = canPlayerInput && Input.GetKeyDown(interactiveBtn);
        movement = canPlayerInput ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized : Vector2.zero;
        leftMouseBtn = canPlayerInput && Input.GetMouseButton(0);
        leftMouseBtnDown = canPlayerInput && Input.GetMouseButtonDown(0);
        leftMouseBtnUp = canPlayerInput && Input.GetMouseButtonUp(0);
        rightMouseBtn = canPlayerInput && Input.GetMouseButton(1);
        rightMouseBtnDown = canPlayerInput && Input.GetMouseButtonDown(1);
        rightMouseBtnUp = canPlayerInput && Input.GetMouseButtonUp(1);
    }
}
