using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    public static Vector3 mousePosition
    {
        get
        {
            return GetMousePosition();
        }
    }
    private static Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; //����� ������� � ��������� z = 0 ��� ������ z = -10
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}