using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealer
{
    float damage { get; set; }
    void DealDamage(float damage, IDamageReceiver target);
}
