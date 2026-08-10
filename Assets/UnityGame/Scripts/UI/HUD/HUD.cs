using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public enum State
    {
        Game,
        Pause,
        GameOver,
        GameComplete
    }
    
    public PlayerInventory inventory;
    public HeathBar healthBar;
    public GameObject equilibriumCharge;
    public CursorBehaviour mouseCursor;

    public Action<State> StateChange;

    private State _state = State.Game;
    public State state
    {
        get => _state;
        set
        {
            _state = value;
            StateChange?.Invoke(_state);
        }
    } 
    
    private void Awake()
    {
        G.HUD = this;
    }
}
