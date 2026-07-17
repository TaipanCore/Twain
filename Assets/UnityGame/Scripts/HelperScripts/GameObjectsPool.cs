using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            pool.Enqueue(CreateNewObject());
        }
    }
    private GameObject CreateNewObject()
    {
        GameObject gameObj = Instantiate(prefab, transform);
        gameObj.SetActive(false);
        return gameObj;
    }
    public GameObject Get(Action<GameObject> GetAction = null)
    {
        GameObject gameObj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
        GetAction?.Invoke(gameObj);
        gameObj.SetActive(true);
        activeObjects.Add(gameObj);
        return gameObj;
    }
    public void Return(GameObject gameObj, Action<GameObject> ReturnAction = null)
    {
        if (!gameObj)
        {
            Debug.LogError("Returnable object already destroyed!");
            return;
        }
        if (activeObjects.Contains(gameObj))
        {
            ReturnAction?.Invoke(gameObj);
            gameObj.SetActive(false);
            activeObjects.Remove(gameObj);
            pool.Enqueue(gameObj);
        }
    }
}
