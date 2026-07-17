using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameSaveLoadManager : MonoBehaviour
{
    private List<ISaveLoadObject> saveLoadObjects = new ();
    
    private SaveLoadSystem saveLoadSystem;

    private void Awake()
    {
        G.gameSaveLoad = this;
        saveLoadSystem = new SaveLoadSystem();
    }

    public void Register(ISaveLoadObject obj)
    {
        saveLoadObjects.Add(obj);
    }

    public void AddAllObjectsToSave()
    {
        foreach (ISaveLoadObject obj in saveLoadObjects)
            saveLoadSystem.AddObjectToSave(obj);
    }

    public void SaveGame(String saveFileName = "DefaultGameSave.json")
    {
        saveLoadSystem.Save(saveFileName);
    }

    public void LoadGame(String saveFileName = "DefaultGameSave.json")
    {
        saveLoadSystem.Load(saveFileName);
    }
}
