using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClotMovement : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float retreatDistance;

    private Coroutine currentBehaviour;
    private Vector3 target;
    private void FixedUpdate()
    {
        target = gameManager.currentCharacter.transform.position;     
        if (gameManager.isUnited)
        {
            ClearBehaviour();
            currentBehaviour = StartCoroutine(AvoidEquilibrium());
        }
        else
        {
            ClearBehaviour();
            currentBehaviour = StartCoroutine(HuntForLightSide());
        }
    }
    private void ClearBehaviour()
    {
        if (currentBehaviour != null)
        {
            StopCoroutine(currentBehaviour);
        }
    }
    IEnumerator HuntForLightSide()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, Time.fixedDeltaTime * moveSpeed);
        yield return null;
    }
    IEnumerator AvoidEquilibrium()
    {        
        transform.position = Vector3.MoveTowards(transform.position, GetRetreatPosition(), Time.fixedDeltaTime * moveSpeed);
        yield return null;
    }
    private Vector3 GetRetreatPosition()
    {
        Vector3 retreatPosition = (transform.position - target).normalized;
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
