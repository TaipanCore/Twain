using UnityEngine;

public static class G
{
    //Managers
    public static CharactersManager characters;
    public static InputManager input;
    public static MouseTracker mouseTracker;
    public static GameSaveLoadManager gameSaveLoad;
    
    //Objects dictionary
    public static ObjectsDictionary objectsDictionary;
    public static EnemiesDieStates enemiesDieStates;

    //HUD
    public static HUD HUD;
    public static GamePause gamePause;
    public static GameOver gameOver;

    //Player & enemy layer masks
    public static LayerMask playerMask => LayerMask.GetMask("Player");
    public static LayerMask enemyMask => LayerMask.GetMask("Enemy");
    
    //Cameras
    public static GameObject mainCamera;
    public static GameObject mapCamera;
}
