using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FireflyMovement : MonoBehaviour
{

    [SerializeField, Min(0)] private float pathPointsSpread;
    [SerializeField, Min(0)] private int pathPointsCount;
    [SerializeField, Min(0)] private float lightRange;
    
    private Vector2 movement;
    private Vector3 previousPosition;
    private Sequence pathSequence;
    private List<Vector3> restOfPath;
    private CircleLight circleLight;
    private FireflySounds fireflySounds;
    private AudioSource fireflyLifeSound;

    private void Start()
    {
        circleLight = transform.GetComponentInChildren<CircleLight>();
        fireflySounds = GetComponent<FireflySounds>();
    }
    private void Update()
    {
        movement = transform.position - previousPosition;
        previousPosition = transform.position;
        FlipSprite();
    }

    private void OnDisable()
    {
        
    }

    public Tween MoveAlongPath(Vector3 endPoint, float pathDuration, Vector3[] path = null)
    {
        path ??= GeneratePath(pathPointsCount, endPoint);
        restOfPath = path.ToList();
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOPath(path, pathDuration, PathType.CatmullRom)
                .OnWaypointChange(waypointIndex =>
                {
                    if (path.Length < waypointIndex)
                        restOfPath.Remove(path[waypointIndex]);
                }))
            .Join(DOVirtual.DelayedCall(Mathf.Clamp(0.5f, 0f, pathDuration), () =>
            {
                fireflyLifeSound = fireflySounds.PlayFireflyLifeSound();
                fireflyLifeSound.transform.parent = transform;
            }, false))
            .Join(DOVirtual.DelayedCall(Mathf.Clamp(pathDuration - 0.5f, 0f, pathDuration), () =>
            {
                fireflyLifeSound.transform.parent = G.audio.transform;
                fireflyLifeSound.Stop();
            }, false))
            .Join(DOVirtual.DelayedCall(Mathf.Clamp(pathDuration - 2f, 0f, pathDuration), () =>
            {
                circleLight.SetRange(0f, 2f);
            }, false))
            .AppendCallback(() => circleLight.SetRange(lightRange));
        pathSequence = sequence;
        sequence.OnComplete(() =>
        {
            pathSequence = null;
        });
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

    [Serializable]
    public class FireflyData
    {
        public FireflyData(Vector3 position, Vector3[] restOfPath, float pathRestOfTime, char color)
        {
            this.position = position;
            this.restOfPath = restOfPath;
            this.pathRestOfTime = pathRestOfTime;
            this.color = color;
        }

        public Vector3 position;
        public Vector3[] restOfPath;
        public float pathRestOfTime;
        public char color;
    }

    public FireflyData PackFireflyData(char color)
    {
        return new FireflyData(
            transform.position,
            restOfPath.ToArray(),
            pathSequence.Duration(false) - pathSequence.Elapsed(false),
            color
        );
    }
}
