using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class EnemiesDieStates : MonoBehaviour, ISaveLoadObject
{
    private Dictionary<String, bool> enemiesDieStates = new ();

    private void Awake()
    {
        G.enemiesDieStates = this;
        RegisterInSaveLoadSystem();
    }

    public void SetDieState(String enemyId)
    {
        enemiesDieStates[enemyId] = true;
    }

    public bool Contains(String enemyId)
    {
        return enemiesDieStates.ContainsKey(enemyId);
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[] { enemiesDieStates });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        enemiesDieStates.Clear();
        //data[0] - enemiesDieStates
        enemiesDieStates = ((JObject)dataToUnpack.data[0]).ToObject<Dictionary<String, bool>>();
        foreach (KeyValuePair<String, bool> pair in enemiesDieStates)
        {
            if (enemiesDieStates[pair.Key])
            {
                Destroy(G.objectsDictionary.Find(pair.Key));
            }
        }
    }
}
