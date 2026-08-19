using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class GameSaveLoadManager : MonoBehaviour
{
    [SerializeField] private WebLoader webLoader;
    
    private List<ISaveLoadObject> saveLoadObjects = new ();
    
    private SaveLoadSystem saveLoadSystem;

    private void Awake()
    {
        G.gameSaveLoad = this;
        saveLoadSystem = new SaveLoadSystem(webLoader);
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

    public void LoadGame(String saveFileName = "DefaultGameSave.json", bool loadEmptySave = false)
    {
        saveLoadSystem.Load(saveFileName, loadEmptySave);
    }
}
