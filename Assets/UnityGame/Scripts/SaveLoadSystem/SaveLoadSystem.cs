using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadSystem
{
    private Dictionary<String, ISaveLoadObject> objectsToSave = new ();
    
    private String savesFolderPath => Path.Combine(Application.persistentDataPath, "Saves");
    private JsonSerializerSettings serializerSettings = new ()
    {
        Formatting = Formatting.Indented
    };
    
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
    public void Load(String saveFileName)
    {
        String fullSaveFilePath = Path.Combine(savesFolderPath, saveFileName);
        if (!File.Exists(fullSaveFilePath))
        {
            Debug.LogError($"Save file {fullSaveFilePath} not found");
            return;
        }
        String serializedSaveFile = File.ReadAllText(fullSaveFilePath);
        SaveFile saveFile = JsonConvert.DeserializeObject<SaveFile>(serializedSaveFile, serializerSettings);
        if (saveFile?.objectsData == null)
        {
            Debug.LogError($"Deserialized file {fullSaveFilePath} is empty");
            return;
        }
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
