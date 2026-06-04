using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode uniteBtn;
    [SerializeField] private KeyCode sidesChangeBtn;
    [SerializeField] private KeyCode interactiveBtn;
    [SerializeField] private KeyCode mapBtn;
    [SerializeField] private KeyCode pauseBtn;

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
    
    public static bool HUDPauseBtnDown;

    private void Update()
    {
        if (canPlayerInput)
        {
            uniteBtnDown = Input.GetKeyDown(uniteBtn);
            sidesChangeBtnDown = Input.GetKeyDown(sidesChangeBtn);
            interactiveBtnDown = Input.GetKeyDown(interactiveBtn);
            mapBtnDown = Input.GetKeyDown(mapBtn);
            leftMouseBtn = Input.GetMouseButton(0);
            leftMouseBtnDown = Input.GetMouseButtonDown(0);
            leftMouseBtnUp = Input.GetMouseButtonUp(0);
            rightMouseBtn = Input.GetMouseButton(1);
            rightMouseBtnDown = Input.GetMouseButtonDown(1);
            rightMouseBtnUp = Input.GetMouseButtonUp(1);
            
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            mouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
        }
        else
        {
            movement = Vector2.zero;
            mouseWheel = 0f;
        }
        HUDPauseBtnDown = Input.GetKeyDown(pauseBtn);
    }
}
