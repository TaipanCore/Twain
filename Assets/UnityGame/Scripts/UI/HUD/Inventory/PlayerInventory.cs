using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour, ISaveLoadObject
{
    [SerializeField] private GameObject itemSlotPrefab;
    
    public Dictionary<GameObject, GameObject> items = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    
    public void AddItem(GameObject item)
    {
        GameObject itemSlot = Instantiate(itemSlotPrefab, itemSlotPrefab.GetComponent<RectTransform>().position, Quaternion.identity, transform);
        itemSlot.GetComponent<Image>().sprite = item.GetComponent<ItemAvatar>().avatarSprite;
        items.Add(item, itemSlot);
        if (item.TryGetComponent(out ShardBehaviour shardBehaviour))
            shardBehaviour.owner = ShardBehaviour.Owner.Inventory;
        item.SetActive(false);
    }
    public void RemoveItem(GameObject item)
    {
        Destroy(items[item]);
        if (item.TryGetComponent(out ShardBehaviour shardBehaviour))
            shardBehaviour.owner = ShardBehaviour.Owner.World;
        items.Remove(item);
    }

    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        List<String> serializedItems = new ();
        foreach (KeyValuePair<GameObject, GameObject> pair in items)
        {
            String itemId = pair.Key.GetComponent<ObjectId>().id;
            serializedItems.Add(itemId);
        }
        return new ObjectSaveLoadData(objectId, new System.Object[] { serializedItems });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        items.Clear();
        //data[0] - items
        List<String> serializedItems = ((JArray)dataToUnpack.data[0]).ToObject<List<String>>();
        foreach (String itemId in serializedItems)
        {
            AddItem(G.objectsDictionary.Find(itemId));
        }
    }
}
