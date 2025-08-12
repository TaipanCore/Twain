using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelBehaviour : MonoBehaviour
{
    [SerializeField] private BatteryBehaviour _batteryBehaviour;
    [SerializeField] private float _panelEfficiency;
    [SerializeField] private ParticleSystem _sparks;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSide" && _batteryBehaviour.batteryCharge < 100f)
        {
            if (_sparks != null && !_sparks.isPlaying)
                _sparks.Play();
            _batteryBehaviour.isCharging = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSide")
            _batteryBehaviour.isCharging = false;
    }
    private void Update()
    {
        if (_batteryBehaviour.isCharging)
        {
            _batteryBehaviour.batteryCharge += Time.deltaTime * _panelEfficiency;
        }
    }
}
