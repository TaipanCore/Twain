using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ShardBehaviour : MonoBehaviour, ISaveLoadObject
{
    public enum Owner
    {
        None,
        Enemy,
        Inventory,
        World,
        FinalGates
    }

    private Owner _owner = Owner.None;
    public Owner owner
    {
        get => _owner;
        set
        {
            _owner = value;
            switch (_owner)
            {
                case Owner.None:
                case Owner.Enemy:
                case Owner.Inventory:
                    gameObject.SetActive(false);
                    break;
                case Owner.World:
                case Owner.FinalGates:
                    gameObject.SetActive(true);
                    break;
            }
        }
    }
    
    private CapsuleCollider2D takeCollider;
    
    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
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
            transform.position,
            owner
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - position
        if (!transform.parent)
            transform.position = ((JObject)dataToUnpack.data[0]).ToObject<Vector3>();
        //data[1] - owner
        if (Enum.TryParse(dataToUnpack.data[1].ToString(), out Owner parsedOwner))
            owner = parsedOwner;
    }
}
