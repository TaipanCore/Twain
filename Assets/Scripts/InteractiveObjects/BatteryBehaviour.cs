using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite[] batterySprites;
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool isCharging;
    private float _batteryCharge;
    [HideInInspector] public float batteryCharge
    {
        get
        {
            return _batteryCharge;
        }
        set
        {
            _batteryCharge = Mathf.Clamp(value, 0f, 100f);
            Debug.Log($"{Mathf.FloorToInt(_batteryCharge)}%");            
        }
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (isCharging)
        {
            if (_batteryCharge < 25f)
                spriteRenderer.sprite = batterySprites[0];
            else if (_batteryCharge < 50f)
                spriteRenderer.sprite = batterySprites[1];
            else if (_batteryCharge < 75f)
                spriteRenderer.sprite = batterySprites[2];
            else if (_batteryCharge < 100f)
                spriteRenderer.sprite = batterySprites[3];
            else
            {
                spriteRenderer.sprite = batterySprites[4];
                isCharging = false;
            }               
        }
    }
}
