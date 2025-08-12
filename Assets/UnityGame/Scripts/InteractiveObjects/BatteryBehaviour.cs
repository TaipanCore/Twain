using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class BatteryBehaviour : MonoBehaviour
{
    [SerializeField] private float chargeDisplayTime = 0.5f;
    [SerializeField] private float chargeHideTime = 1f;
    private WaitForSeconds shortDelay;
    private WaitForSeconds longDelay;
    private Coroutine blinking;

    [SerializeField] private Sprite[] batterySprites;
    private Sprite currentSprite;
    private SpriteRenderer spriteRenderer;

    private bool _isCharging;
    [HideInInspector] public bool isCharging
    {
        get => _isCharging;
        set
        {
            _isCharging = value;
            if (_isCharging)
            {
                if (blinking == null)
                    blinking = StartCoroutine(SpriteBlinking());
            }               
            else
            {
                if (blinking != null)
                {
                    StopCoroutine(blinking);
                    blinking = null;                  
                }
                spriteRenderer.sprite = currentSprite;
            }
        }
    }
    private float _batteryCharge;
    [HideInInspector] public float batteryCharge
    {
        get => _batteryCharge;
        set
        {
            _batteryCharge = Mathf.Clamp(value, 0f, 100f);
            //Debug.Log($"{Mathf.FloorToInt(_batteryCharge)}%");
        }
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shortDelay = new WaitForSeconds(chargeDisplayTime);
        longDelay = new WaitForSeconds(chargeHideTime);
        currentSprite = batterySprites[1];
    }
    private void Update()
    {
        if (_isCharging)
        {
            currentSprite = batterySprites[Mathf.FloorToInt(_batteryCharge / 25f) + 1];
            if (currentSprite == batterySprites[batterySprites.Length - 1])
                isCharging = false;
        }
    }
    private IEnumerator SpriteBlinking()
    {
        while (true)
        {
            spriteRenderer.sprite = currentSprite;
            yield return shortDelay;
            spriteRenderer.sprite = batterySprites[0];
            yield return longDelay;      
        }
    }
}
