using System.Collections;
using UnityEngine;

public class WanderingClotBehaviour : ClotBehaviour
{
    private Coroutine wanderingCoroutine;
    private WanderingClotMovement wanderingMovement => movement as WanderingClotMovement;
    
    protected override void SetIdleSettings()
    {
        base.SetIdleSettings();
        wanderingMovement.SetWanderingMoveSpeed();
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
                movement.navMeshAgent.SetDestination(wanderingMovement.GetWanderingPosition());
            }
            yield return null;
        }
    }
}

