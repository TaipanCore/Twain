using UnityEngine;

public class SquidShrineBehaviour : MonoBehaviour
{
    [SerializeField] SquidBossBehaviour squidBossBehaviour;
    [SerializeField] CapsuleCollider2D activationCollider;
    
    private Transform fireflyPoint;
    private Transform firefly;
    private Vector3 fireflyOldLocalPosition;
    private void Start()
    {
        fireflyPoint = transform.Find("FireflyPoint");
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (InputManager.interactiveBtnDown)
        {
            if (other.TryGetComponent(out LightSideBehaviour lightSideBehaviour))
            {
                if (!firefly && squidBossBehaviour)
                {
                    firefly = lightSideBehaviour.TakeFirefly(fireflyPoint);
                    squidBossBehaviour.StartBattle();
                }
                else if (squidBossBehaviour.bossDefeated)
                {
                    lightSideBehaviour.ReturnFirefly();
                    activationCollider.enabled = false;
                }
            }
        }
    }
}
