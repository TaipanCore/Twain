using System.Collections;
using UnityEngine;

public class WanderingClotBehaviour : ClotBehaviour
{
    private Coroutine wanderingCoroutine;
    private WanderingClotMovement wanderingMovement => movement as WanderingClotMovement;
    
    protected override void SetIdleSettings()
    {
        isAggro = false;
        wanderingMovement.SetMoveState(State.Idle);
        wanderingCoroutine ??= StartCoroutine(WanderingCoroutine());
    }

    protected override void Idle()
    {
        if (isAggro)
        {
            StopCoroutine(wanderingCoroutine);
            wanderingCoroutine = null;
            SetState(State.Hunt);
        }
    }
    
    private IEnumerator WanderingCoroutine()
    {
        while (true)
        {
            if (!Utils.IsAgentMoving(movement.navMeshAgent))
            {
                movement.navMeshAgent.SetPath(wanderingMovement.GetOnCirclePositionForCurrentState());
            }
            yield return null;
        }
    }
}

