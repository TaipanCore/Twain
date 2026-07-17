using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    public Vector3 mousePosition => GetMousePosition();

    private void Awake()
    {
        G.mouseTracker = this;
    }
    
    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; //����� ������� � ��������� z = 0 ��� ������ z = -10
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}