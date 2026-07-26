using UnityEngine;

public class UICursorFollower : MonoBehaviour
{
    private RectTransform objTransform;
    private Canvas canvas;
    private RectTransform canvasTransform;
    private Vector2 mousePos;
    
    protected virtual void Start()
    {
        objTransform = GetComponent<RectTransform>();
        canvas = transform.GetComponentInParent<Canvas>();
        canvasTransform = canvas.GetComponent<RectTransform>();
    }
    protected virtual void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        objTransform.localPosition = mousePos;
    }
}
