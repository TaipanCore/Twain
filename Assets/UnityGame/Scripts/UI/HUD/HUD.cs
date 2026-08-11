using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public enum State
    {
        Game,
        Pause,
        Map,
        GameOver,
        GameComplete
    }
    
    public PlayerInventory inventory;
    public HeathBar healthBar;
    public GameObject equilibriumCharge;
    public CursorBehaviour mouseCursor;

    public Action<State> StateChange;

    private State _state;
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
        state = State.Game;
    }
}
