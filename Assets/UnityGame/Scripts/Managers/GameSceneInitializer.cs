using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        G.gameSaveLoad.AddAllObjectsToSave();
        if (G.isDefaultGameSaveExists)
            G.gameSaveLoad.LoadGame();
        else
            G.gameSaveLoad.LoadGame("EmptyGameSave.json");
        
        G.gamePause.Resume();
    }
}
