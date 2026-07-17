using UnityEngine;

public class HUD : MonoBehaviour
{
    public enum State
    {
        Game,
        Pause,
        GameOver
    }
    
    public PlayerInventory inventory;
    public HeathBar healthBar;
    public GameObject equilibriumCharge;
    public CursorBehaviour mouseCursor;
    public State state = State.Game;
    
    private void Awake()
    {
        G.HUD = this;
    }
}
