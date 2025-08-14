using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClotMovement : MonoBehaviour
{
    private enum Behaviour
    {
        Idle,
        Hunt,
        Retreat
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float retreatDistance;

    private Behaviour behaviour;
    private Vector3 target;

    private void Start()
    {
        SetBehaviour(Behaviour.Hunt);
    }
    private void FixedUpdate()
    {
        target = GameManager.currentCharacter.transform.position;
        switch (behaviour)
        {
            case Behaviour.Idle:
                break;
            case Behaviour.Hunt:
                HuntForLightSide();
                break;
            case Behaviour.Retreat:
                AvoidEquilibrium();
                break;           
        }
    }

    private void SetBehaviour(Behaviour newBehaviour)
    {
        behaviour = newBehaviour;
    }
    private void HuntForLightSide()
    {
        if (GameManager.isUnited)
        {
            SetBehaviour(Behaviour.Retreat);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, Time.fixedDeltaTime * moveSpeed);
    }
    private void AvoidEquilibrium()
    {
        if (!GameManager.isUnited)
        {
            SetBehaviour(Behaviour.Hunt);
        }
        transform.position = Vector3.MoveTowards(transform.position, GetRetreatPosition(), Time.fixedDeltaTime * moveSpeed);
    }
    private Vector3 GetRetreatPosition()
    {
        Vector3 retreatPosition = (transform.position - target).normalized;
        if (retreatPosition == Vector3.zero)
            retreatPosition = Random.onUnitSphere;
        return target + retreatPosition * retreatDistance;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, GetRetreatPosition());
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target, retreatDistance);
    }
    */
}
