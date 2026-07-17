using System;
using UnityEngine;

public class ObjectId : MonoBehaviour
{
    public String id;
    
    private void Awake()
    {
        if (!String.IsNullOrWhiteSpace(id))
            G.objectsDictionary.Register(id, gameObject);
        else
            Debug.LogError($"Object {gameObject.name} has empty id!");
    }
    private void OnDestroy()
    {
        G.objectsDictionary.Unregister(id);
    }
}
