using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class BatteryBehaviour : MonoBehaviour
{
    [SerializeField] private float chargeDisplayTime = 0.5f;
    [SerializeField] private float chargeHideTime = 1f;
    private WaitForSeconds _shortDelay;
    private WaitForSeconds _longDelay;
    private Coroutine _blinking;

    [SerializeField] private Sprite[] _batterySprites;
    private Sprite _currentSprite;
    private SpriteRenderer _spriteRenderer;

    private bool _isCharging;
    [HideInInspector] public bool isCharging
    {
        get => _isCharging;
        set
        {
            _isCharging = value;
            if (_isCharging)
            {
                if (_blinking == null)
                    _blinking = StartCoroutine(SpriteBlinking());
            }               
            else
            {
                if (_blinking != null)
                {
                    StopCoroutine(_blinking);
                    _blinking = null;                  
                }
                _spriteRenderer.sprite = _currentSprite;
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
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shortDelay = new WaitForSeconds(chargeDisplayTime);
        _longDelay = new WaitForSeconds(chargeHideTime);
        _currentSprite = _batterySprites[1];
    }
    private void Update()
    {
        if (_isCharging)
        {
            _currentSprite = _batterySprites[Mathf.FloorToInt(_batteryCharge / 25f) + 1];
            if (_currentSprite == _batterySprites[_batterySprites.Length - 1])
                isCharging = false;
        }
    }
    private IEnumerator SpriteBlinking()
    {
        while (true)
        {
            _spriteRenderer.sprite = _currentSprite;
            yield return _shortDelay;
            _spriteRenderer.sprite = _batterySprites[0];
            yield return _longDelay;      
        }
    }
}
