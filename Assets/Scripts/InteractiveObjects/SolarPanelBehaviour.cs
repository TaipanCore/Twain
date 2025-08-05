using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelBehaviour : MonoBehaviour
{
    [SerializeField] private BatteryBehaviour batteryBehaviour;
    [SerializeField] private float panelEfficiency;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSide" && batteryBehaviour.batteryCharge < 100f)
            batteryBehaviour.isCharging = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LightSide")
            batteryBehaviour.isCharging = false;
    }
    private void Update()
    {
        if (batteryBehaviour.isCharging)
        {
            batteryBehaviour.batteryCharge += Time.deltaTime * panelEfficiency;
        }
    }
}
