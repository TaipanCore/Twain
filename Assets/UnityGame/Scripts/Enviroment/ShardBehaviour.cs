using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ShardBehaviour : MonoBehaviour, ISaveLoadObject
{
    private CapsuleCollider2D takeCollider;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        takeCollider = GetComponent<CapsuleCollider2D>();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (G.input.interactiveBtnDown)
        {
            if (other.gameObject.IsInLayerMask(G.playerMask) && takeCollider.enabled)
            {
                GetComponent<ShardSounds>().PlayPickUpSound();
                takeCollider.enabled = false;
                G.HUD.inventory.AddItem(gameObject);
            }
        }
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            transform.position
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - position
        if (!transform.parent)
            transform.position = ((JObject)dataToUnpack.data[0]).ToObject<Vector3>();
    }
}
