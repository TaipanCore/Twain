using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Key bindings")]
    [SerializeField] private KeyCode uniteBtn;
    [SerializeField] private KeyCode sidesChangeBtn;
    [SerializeField] private KeyCode interactiveBtn;
    [SerializeField] private KeyCode mapBtn;
    [SerializeField] private KeyCode pauseBtn;

    [HideInInspector] public bool canPlayerInput = true;
        
    [HideInInspector] public bool uniteBtnDown;
    [HideInInspector] public bool sidesChangeBtnDown;
    [HideInInspector] public bool interactiveBtnDown;
    [HideInInspector] public bool mapBtnDown;
    [HideInInspector] public Vector2 movement;
    [HideInInspector] public bool leftMouseBtn;
    [HideInInspector] public bool leftMouseBtnDown;
    [HideInInspector] public bool leftMouseBtnUp;
    [HideInInspector] public bool rightMouseBtn;
    [HideInInspector] public bool rightMouseBtnDown;
    [HideInInspector] public bool rightMouseBtnUp;
    [HideInInspector] public float mouseWheel;
    
    [HideInInspector] public bool HUDPauseBtnDown;
    [HideInInspector] public bool HUDAnyKeyDown;

    private void Awake()
    {
        G.input = this;
    }
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
        HUDAnyKeyDown = Input.anyKeyDown;
    }
}
