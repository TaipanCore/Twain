using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsPool<T>
{
    private Queue<T> pool = new ();
    private HashSet<T> activeObjects = new ();

    private Func<T> CreateNewObject;

    public ObjectsPool(int poolSize, Func<T> CreateNewObject)
    {
        this.CreateNewObject = CreateNewObject;
        for (int i = 0; i < poolSize; i++)
            pool.Enqueue(CreateNewObject());
    }
    
    public T Get(Action<T> GetAction = null)
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
        GetAction?.Invoke(obj);
        activeObjects.Add(obj);
        return obj;
    }
    public void Return(T obj, Action<T> ReturnAction = null)
    {
        if (obj == null)
        {
            Debug.LogError("Returnable object doesn't exist!");
            return;
        }
        if (activeObjects.Contains(obj))
        {
            ReturnAction?.Invoke(obj);
            activeObjects.Remove(obj);
            pool.Enqueue(obj);
        }
    }

    public HashSet<T> GetActiveObjects()
    {
        return activeObjects;
    }
}