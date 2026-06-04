using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject itemSlotPrefab;
    
    public Dictionary<GameObject, GameObject> items = new Dictionary<GameObject, GameObject>();
    
    public void AddItem(GameObject item)
    {
        GameObject itemSlot = Instantiate(itemSlotPrefab, itemSlotPrefab.GetComponent<RectTransform>().position, Quaternion.identity, transform);
        itemSlot.GetComponent<Image>().sprite = item.GetComponent<ItemAvatar>().avatarSprite;
        items.Add(item, itemSlot);
    }
    public void RemoveItem(GameObject item)
    {
        Destroy(items[item]);
        items.Remove(item);
    }
}
