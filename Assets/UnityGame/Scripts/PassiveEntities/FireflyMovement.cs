using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FireflyMovement : MonoBehaviour
{

    [SerializeField, Min(0)] private float pathPointsSpread;
    [SerializeField, Min(0)] private int pathPointsCount;
    [SerializeField, Min(0)] private float lightRange;
    
    private Vector2 movement;
    private Vector3 previousPosition;
    private CircleLight circleLight;

    private void Awake()
    {
        circleLight = transform.GetComponentInChildren<CircleLight>();
    }
    private void Update()
    {
        movement = transform.position - previousPosition;
        previousPosition = transform.position;
        FlipSprite();
    }

    public Tween MoveAlongPath(Vector3 endPoint, float pathDuration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOPath(GeneratePath(pathPointsCount, endPoint), pathDuration, PathType.CatmullRom))
            .Join(DOVirtual.DelayedCall(pathDuration - 2f, () => circleLight.SetRange(0f, 2f)))
            .AppendCallback(() => circleLight.SetRange(lightRange));
        return sequence;
    }
    private Vector3[] GeneratePath(int pointsCount, Vector3 endPosition)
    {
        Vector3[] path = new Vector3[pointsCount];
        path[pointsCount - 1] = endPosition;
        Vector3 direction = endPosition - transform.position;
        Vector2 stepVector = new Vector2(direction.x, direction.y) / pointsCount;
        for (int i = 0; i < pointsCount - 1; i++)
        {
            path[i] = transform.position + (Vector3)(stepVector * i + Random.insideUnitCircle * pathPointsSpread);
        }
        return path;
    }
    private void FlipSprite()
    {
        transform.rotation = Quaternion.Euler(0, movement.x > 0f ? 0 : 180, 0);
    }
}
