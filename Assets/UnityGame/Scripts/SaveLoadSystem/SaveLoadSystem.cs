using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class SaveLoadSystem
{
    private WebLoader webLoader;
    
    private Dictionary<String, ISaveLoadObject> objectsToSave = new ();
    
    private String savesFolderPath => Path.Combine(Application.persistentDataPath, "Saves");
    private String emptySaveFilePath => Path.Combine(Application.streamingAssetsPath, "EmptyGameSave.json");
    private JsonSerializerSettings serializerSettings = new ()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public SaveLoadSystem(WebLoader webLoader)
    {
        this.webLoader = webLoader;
    }
    
    public void AddObjectToSave(ISaveLoadObject saveLoadObject)
    {
        objectsToSave[saveLoadObject.objectId] = saveLoadObject;
    }
    
    public void Save(String saveFileName)
    {
        List<ObjectSaveLoadData> serializedData = new List<ObjectSaveLoadData>();
        foreach (String objectId in objectsToSave.Keys)
        {
            if (!G.enemiesDieStates.Contains(objectId))
            {
                GameObject gameObject = G.objectsDictionary.Find(objectId);
                if (gameObject)
                {
                    if (!gameObject.activeInHierarchy)
                        gameObject.SetActive(true);
                    serializedData.Add(objectsToSave[objectId].PackData());
                }
                else
                    Debug.LogWarning($"Object with id \"{objectId}\" removed from save: object doesn't exist!");
            }
        }
        if (!Directory.Exists(savesFolderPath))
            Directory.CreateDirectory(savesFolderPath);
        SaveFile saveFile = new SaveFile(serializedData);
        String serializedSaveFile = JsonConvert.SerializeObject(saveFile, serializerSettings);
        File.WriteAllText(Path.Combine(savesFolderPath, saveFileName), serializedSaveFile);
    }
    public void Load(String saveFileName, bool loadEmptySave)
    {
        String fullSaveFilePath = loadEmptySave ? emptySaveFilePath : Path.Combine(savesFolderPath, saveFileName);
        if (loadEmptySave)
        {
            #if UNITY_WEBGL
                webLoader.LoadSaveUsingWebRequest(fullSaveFilePath, DeserializeSaveFile);
            #else
                DeserializeSaveFile(File.ReadAllText(fullSaveFilePath));
            #endif
        }
        else
        {
            if (!File.Exists(fullSaveFilePath))
            {
                Debug.LogError($"Save file {fullSaveFilePath} not found");
                return;
            }
            DeserializeSaveFile(File.ReadAllText(fullSaveFilePath));
        }
    }

    public void DeserializeSaveFile(String serializedSaveFile)
    {
        SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(serializedSaveFile, serializerSettings);
        foreach (ObjectSaveLoadData data in saveFile.objectsData)
        {
            if (!objectsToSave.ContainsKey(data.dataObjectId))
            {
                Debug.LogError($"Can't unpack data for object with id: {data.dataObjectId}");
                continue;
            }
            objectsToSave[data.dataObjectId].UnpackData(data);
        }
    }
    
    
}
