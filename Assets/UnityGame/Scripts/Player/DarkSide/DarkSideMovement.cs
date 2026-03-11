using System.Collections;
using UnityEngine;

public class DarkSideMovement : PlayerMovement
{
    [SerializeField, Range(1, 100)] private float minFollowRadiusPercents;
    [SerializeField, Range(1, 100)] private float maxFollowRadiusPercents;

    private Transform objectTransform;
    private Transform fireflyTransform;
    private LightSideBehaviour lightSideBehaviour;
    
    private float minFollowRadius;
    private float maxFollowRadius;

    protected override void Start()
    {
        base.Start();
        objectTransform = GetComponent<Transform>();
        fireflyTransform = GameManager.lightSide.transform.Find("Firefly");
        lightSideBehaviour = GameManager.lightSide.GetComponent<LightSideBehaviour>();
    }
    protected override void FixedUpdate()
    {
        if (GameManager.currentCharacter == gameObject)
        {
            movementVector = InputManager.movement;
            rb.MovePosition(rb.position + movementVector * (moveSpeed * Time.fixedDeltaTime));
        }
        else if (GameManager.currentCharacter == GameManager.lightSide)
        {
            movementVector = (fireflyTransform.position - objectTransform.position).normalized;
            minFollowRadius = lightSideBehaviour.GetCurrentLightRange() * minFollowRadiusPercents / 100f;
            maxFollowRadius = lightSideBehaviour.GetCurrentLightRange() * maxFollowRadiusPercents / 100f;
            if (!Utils.IsInRange(objectTransform.position, fireflyTransform.position, maxFollowRadius))
            {
                StartCoroutine(MoveToFirefly());
            }
        }
        currentSpeed = (movementVector * moveSpeed).magnitude;
        FlipCharacter();
    }
    private IEnumerator MoveToFirefly()
    {
        while (!Utils.IsInRange(objectTransform.position, fireflyTransform.position, minFollowRadius))
        {
            rb.MovePosition(Vector3.MoveTowards(objectTransform.position, fireflyTransform.position, moveSpeed * Time.fixedDeltaTime));
            yield return null;
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (objectTransform)
            Gizmos.DrawLine(objectTransform.position, fireflyTransform.position);
        if (fireflyTransform)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(fireflyTransform.position, minFollowRadius);
            Gizmos.DrawWireSphere(fireflyTransform.position, maxFollowRadius);
        }
    }*/
}
