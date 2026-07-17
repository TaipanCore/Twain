using System;
using UnityEngine;

public class SleepingTorchBehaviour : MonoBehaviour, IAbleAggro, ISaveLoadObject
{
    private bool _isAggro;
    public bool isAggro
    {
        get => _isAggro;
        set
        {
            if (!_isAggro && value)
            {
                _isAggro = true;
                lightSource.SetActive(true);
                animator.Restart();
            }
        }
    }
    
    private SimpleAnimator animator;
    private GameObject lightSource;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        animator = GetComponent<SimpleAnimator>();
        lightSource = transform.GetChild(0).gameObject;
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[] { isAggro });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - isAggro
        if(bool.TryParse(dataToUnpack.data[0].ToString(), out var parsedIsAggro)) 
            isAggro = parsedIsAggro;
    }
}
