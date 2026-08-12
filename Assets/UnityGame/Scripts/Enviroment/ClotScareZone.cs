using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class ClotScareZone : MonoBehaviour
{
    [SerializeField] protected float scaredMoveSpeed;
    [SerializeField] private float minScareMovingDistance;
    [SerializeField] private float maxScareMovingDistance;
    [SerializeField] private int scarePointsOnCircle;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ClotBehaviour behaviour))
        {
            if (!behaviour.isIgnoreLight)
            {
                StartCoroutine(behaviour.IgnoreLight(5f));
                ClotMovement movement = behaviour.GetComponent<ClotMovement>();
                movement.navMeshAgent.speed = scaredMoveSpeed;
                NavMeshPath retreatPath = movement.GetOnCircleNavMeshPosition(Random.Range(minScareMovingDistance, maxScareMovingDistance), scarePointsOnCircle);
                movement.navMeshAgent.SetPath(retreatPath);
            }
        }
    }
}
