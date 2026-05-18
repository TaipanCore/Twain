using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode uniteBtn;
    [SerializeField] private KeyCode sidesChangeBtn;
    [SerializeField] private KeyCode interactiveBtn;
    [SerializeField] private KeyCode mapBtn;

    public static bool canPlayerInput;
        
    public static bool uniteBtnDown;
    public static bool sidesChangeBtnDown;
    public static bool interactiveBtnDown;
    public static bool mapBtnDown;
    public static Vector2 movement;
    public static bool leftMouseBtn;
    public static bool leftMouseBtnDown;
    public static bool leftMouseBtnUp;
    public static bool rightMouseBtn;
    public static bool rightMouseBtnDown;
    public static bool rightMouseBtnUp;
    public static float mouseWheel;

    private void Update()
    {
        uniteBtnDown = canPlayerInput && Input.GetKeyDown(uniteBtn);
        sidesChangeBtnDown = canPlayerInput && Input.GetKeyDown(sidesChangeBtn);
        interactiveBtnDown = canPlayerInput && Input.GetKeyDown(interactiveBtn);
        mapBtnDown = canPlayerInput && Input.GetKeyDown(mapBtn);
        movement = canPlayerInput ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized : Vector2.zero;
        leftMouseBtn = canPlayerInput && Input.GetMouseButton(0);
        leftMouseBtnDown = canPlayerInput && Input.GetMouseButtonDown(0);
        leftMouseBtnUp = canPlayerInput && Input.GetMouseButtonUp(0);
        rightMouseBtn = canPlayerInput && Input.GetMouseButton(1);
        rightMouseBtnDown = canPlayerInput && Input.GetMouseButtonDown(1);
        rightMouseBtnUp = canPlayerInput && Input.GetMouseButtonUp(1);
        mouseWheel = canPlayerInput ? Input.GetAxisRaw("Mouse ScrollWheel") : 0f;
    }
}
