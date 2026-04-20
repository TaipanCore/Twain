using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FireflyMovement : MonoBehaviour
{

    [SerializeField] private float pathPointsSpread;
    [SerializeField] private int pathPointsCount;
    [SerializeField] private float pathDuration;
    [SerializeField] private Transform pathEndPoint;
    
    private Vector2 movement;
    private Vector3 previousPosition;

    private void Start()
    {
        transform.DOPath(GeneratePath(pathPointsCount, pathEndPoint.position), pathDuration, PathType.CatmullRom, gizmoColor:Color.yellow);
    }
    private void Update()
    {
        movement = transform.position - previousPosition;
        previousPosition = transform.position;
        FlipSprite();
    }

    private Vector3[] GeneratePath(int pointsCount, Vector3 endPosition)
    {
        Vector3[] path = new Vector3[pointsCount];
        path[pointsCount - 1] = endPosition;
        Vector3 direction = endPosition - transform.position;
        Vector3 stepVector = new Vector2(direction.x, direction.y) / pointsCount;
        for (int i = 0; i < pointsCount - 1; i++)
        {
            path[i] = transform.position + stepVector * i + Random.insideUnitSphere * pathPointsSpread;
        }
        return path;
    }
    private void FlipSprite()
    {
        transform.rotation = Quaternion.Euler(0, movement.x > 0f ? 180 : 0, 0);
    }
}
