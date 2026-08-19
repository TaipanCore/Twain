using System;
using DG.Tweening;
using UnityEngine;

public class SleepingTorchBehaviour : TorchBehaviour, IAbleAggro, ISaveLoadObject
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
                PlayFiringTorchSounds();
            }
        }
    }
    
    private SimpleAnimator animator;

    protected override void Awake()
    {
        RegisterInSaveLoadSystem();
        
        animator = GetComponent<SimpleAnimator>();
        lightSource = transform.GetChild(0).gameObject;
    }
    
    private void PlayFiringTorchSounds()
    {
        TorchSounds torchSounds = GetComponent<TorchSounds>();
        AudioSource firingSound = torchSounds.PlayTorchFiringSound(lightSource.transform.position);
        DOVirtual.DelayedCall(firingSound.clip.length, () => GetComponent<TorchSounds>().PlayTorchBurningSound(lightSource.transform.position), false);
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
