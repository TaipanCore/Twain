using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ClotScareZone : MonoBehaviour
{
    [SerializeField] protected float scaredMoveSpeed;
    [SerializeField] private float scareMovingDistance;
    [SerializeField] private int scarePointsOnCircle;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ClotBehaviour behaviour))
        {
            if (!behaviour.isIgnoreLight)
            {
                StartCoroutine(behaviour.IgnoreLight(5f));
                ClotMovement movement = behaviour.GetComponent<ClotMovement>();
                movement.navMeshAgent.speed = scaredMoveSpeed;
                movement.navMeshAgent.SetPath(movement.GetOnCircleNavMeshPosition(scareMovingDistance, scarePointsOnCircle));
                StartCoroutine(WaitForEndOfPath(movement.navMeshAgent, movement));
            }
        }
    }

    private IEnumerator WaitForEndOfPath(NavMeshAgent agent, ClotMovement movement)
    {
        yield return new WaitUntil(() => !Utils.IsAgentMoving(agent));
        movement.SetMoveState(ClotBehaviour.State.Idle);
    }
}
