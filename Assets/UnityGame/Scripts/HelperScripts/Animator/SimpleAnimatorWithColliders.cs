using UnityEngine;

public class SimpleAnimatorWithColliders : SimpleAnimator
{
    [SerializeField] private Collider2D[] colliders;

    private Collider2D colliderToDisable;

    protected override void FrameActions()
    {
        base.FrameActions();
        if (colliderToDisable)
            colliderToDisable.enabled = false;
        colliderToDisable = colliders[currentFrame];
        colliders[currentFrame].enabled = true;
    }
}
