using UnityEngine;
using UnityEngine.Rendering;

public class SpriteOrderInLayerController : StaticSpriteOrderInLayerController
{
    private SpriteRenderer spriteRenderer;
    private SortingGroup sortingGroup;

    protected override void Start()
    {
        if (!TryGetComponent(out sortingGroup))
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void LateUpdate()
    {
        Vector3 position = useParentPosition ? transform.parent.position : transform.position;
        int order = Mathf.RoundToInt(-position.y * orderMultiplier);
        if (sortingGroup)
            sortingGroup.sortingOrder = order;
        else
            spriteRenderer.sortingOrder = order;
    }
}
