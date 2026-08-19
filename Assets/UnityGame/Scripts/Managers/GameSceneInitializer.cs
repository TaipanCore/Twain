using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    private void Start()
    {
        G.audio.PlayMusic(G.music.labyrinthMusic);
        
        G.gameSaveLoad.AddAllObjectsToSave();
        if (G.isDefaultGameSaveExists)
            G.gameSaveLoad.LoadGame();
        else
            G.gameSaveLoad.LoadGame(loadEmptySave: true);
        
        G.gamePause.Resume();
    }
}
