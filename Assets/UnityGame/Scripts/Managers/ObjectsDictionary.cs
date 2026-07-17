using System;
using System.Collections.Generic;
using UnityEngine;
public class ObjectsDictionary : MonoBehaviour
{
    private Dictionary<String, GameObject> idToObject = new ();

    private void Awake()
    {
        G.objectsDictionary = this;
    }
    
    public void Register(String id, GameObject obj)
    {
        if (!idToObject.TryAdd(id, obj))
            Debug.LogError($"Objects dictionary already contains object with id {id}");
    }
    public void Unregister(String id)
    {
        idToObject.Remove(id);
    }
    public GameObject Find(String id)
    {
        if (!idToObject.TryGetValue(id, out var obj))
            Debug.LogError($"Objects dictionary doesn't contain object with id {id}");
        return obj;
    }
}
