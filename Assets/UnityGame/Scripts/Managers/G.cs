using UnityEngine;

public static class G
{
    //Managers
    public static CharactersManager characters;
    public static InputManager input;
    public static AudioManager audio;
    public static MouseTracker mouseTracker;
    public static GameSaveLoadManager gameSaveLoad;
    
    //Objects dictionary
    public static ObjectsDictionary objectsDictionary;
    public static EnemiesDieStates enemiesDieStates;

    //HUD
    public static HUD HUD;
    public static GamePause gamePause;
    public static GameOver gameOver;
    
    //Saves
    public static bool isDefaultGameSaveExists;

    //Layer masks
    public static LayerMask playerMask => LayerMask.GetMask("Player");
    public static LayerMask enemyMask => LayerMask.GetMask("Enemy");
    public static LayerMask highObjectsMask => LayerMask.GetMask("HighObject");
    
    //Cameras
    public static GameObject mainCamera;
    public static GameObject mapCamera;
}
