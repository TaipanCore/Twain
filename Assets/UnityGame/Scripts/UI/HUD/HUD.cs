using UnityEngine;

public class HUD : MonoBehaviour
{
    public PlayerInventory inventory;
    public HeathBar healthBar;
    public GameObject equilibriumCharge;
    public CursorBehaviour mouseCursor;
    
    private void Awake()
    {
        GameManager.HUD = this;
    }
}
