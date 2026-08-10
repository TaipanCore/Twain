using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class StaticSpriteOrderInLayerController : MonoBehaviour
{
    [SerializeField] protected bool useParentPosition;
    
    protected static readonly int orderMultiplier = 10;
    
    protected virtual void Start()
    {
        Vector3 position = useParentPosition ? transform.parent.position : transform.position;
        int order = Mathf.RoundToInt(-position.y * orderMultiplier);
        if (TryGetComponent(out SortingGroup sortingGroup))
            sortingGroup.sortingOrder = order;
        else
            GetComponent<SpriteRenderer>().sortingOrder = order;
    }
}
