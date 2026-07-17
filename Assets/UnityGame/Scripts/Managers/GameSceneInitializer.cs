using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        G.gameSaveLoad.AddAllObjectsToSave();
        G.gameSaveLoad.LoadGame();
        
        G.gamePause.Resume();
    }
}
