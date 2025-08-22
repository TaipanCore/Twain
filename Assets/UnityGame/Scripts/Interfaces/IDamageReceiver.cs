using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageReceiver
{
    float hitpoints { get; set; }
    void ReceiveDamage(float damage);
    void Die();
}
