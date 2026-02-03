using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageReceiver
{
    float hitpoints { get; set; }
    void ReceiveDamage(float damage);
    float invulnerableTime { get; set; }
    bool isInvulnerable { get; set; }
    IEnumerator GiveInvulnerability();
    void Die();
}
