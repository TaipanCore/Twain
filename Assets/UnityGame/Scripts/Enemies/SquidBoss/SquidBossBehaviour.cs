using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquidBossBehaviour : MonoBehaviour
{
    [Header("Eyes")]
    [SerializeField] private GameObject[] eyes;
    [SerializeField, Min(0)] private float eyeAppearDelay;
    private WaitForSeconds eyeAppearTimer;
    [Header("Tentacles")]
    [SerializeField] private TentacleBehaviour[] tentacles;
    [SerializeField] private AnimationCurve attackDifficultyOverTime;
    [SerializeField, Min(1)] private int numberOfTentaclesToDefeatBoss;
    [SerializeField, Min(0)] private float timeToReachTarget;
    [SerializeField, Min(0)] private float timeToRetreat;
    [SerializeField, Min(0)] private float minBossWavesDelay;
    [SerializeField, Min(0)] private float maxBossWavesDelay;
    [SerializeField, Min(0)] private float maxDelayBetweenTentaclesAttacks;
    [Header("Boss reward")]
    [SerializeField] private GameObject reward;
    
    public bool bossDefeated {get; private set;}
    
    private int currentNumberOfTentacles;
    private ParticleSystem soundWaveParticles;

    private void Start()
    {
        eyeAppearTimer = new WaitForSeconds(eyeAppearDelay);
        soundWaveParticles =  GetComponent<ParticleSystem>();
    }
    public void StartBattle()
    {
        StartCoroutine(SpawnEyes());
    }
    private void EndBattle()
    {
        bossDefeated = true;
        RetreatAllTentacles();
        HideEyes();
    }

    private void RetreatAllTentacles()
    {
        foreach (TentacleBehaviour tentacle in tentacles)
        {
            tentacle.OnDefeated -= OnTentacleDefeated;
            tentacle.Retreat();
            DOVirtual.DelayedCall(timeToRetreat, () => tentacle.gameObject.SetActive(false));
        }
    }

    private void HideEyes()
    {
        transform.DOPunchPosition(Vector3.up * 0.35f, 1f, 4)
            .OnComplete(() =>
            {
                foreach (GameObject eye in eyes)
                {
                    eye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);
                }
                DOVirtual.DelayedCall(2f, DropReward).OnComplete(() => Destroy(gameObject));
            });
    }
    private void DropReward()
    {
        reward.SetActive(true);
        reward.transform.position = transform.position;
        Vector2 jumpToPosition = transform.position + (Vector3)Random.insideUnitCircle.normalized * 2f;
        ParticleSystem trailParticles = reward.GetComponent<ParticleSystem>();
        reward.transform.DOJump(jumpToPosition, 2f, 1, 0.6f).SetEase(Ease.Linear).OnComplete(() => trailParticles.Stop());
    }
    private IEnumerator SpawnEyes()
    {
        InputManager.canPlayerInput = false;
        foreach (GameObject eye in eyes)
        {
            eye.SetActive(true);
            yield return eyeAppearTimer;
        }
        soundWaveParticles.Play();
    }
    private void OnParticleSystemStopped()
    {
        InputManager.canPlayerInput = true;
        StartCoroutine(TentaclesAttack());
    }
    private IEnumerator TentaclesAttack()
    {
        foreach (TentacleBehaviour tentacle in tentacles)
        {
            tentacle.gameObject.SetActive(true);
            tentacle.OnDefeated += OnTentacleDefeated;
        }
        while (!bossDefeated)
        {
            int tentaclesInWaveCount = Mathf.CeilToInt(tentacles.Length * attackDifficultyOverTime.Evaluate((float)currentNumberOfTentacles / numberOfTentaclesToDefeatBoss));
            foreach (TentacleBehaviour tentacle in Utils.GetRandomElements(tentacles, tentaclesInWaveCount))
            {
                tentacle.timeToReachTarget = timeToReachTarget;
                tentacle.timeToRetreat = timeToRetreat;
                tentacle.Attack();
                yield return new WaitForSeconds(Random.Range(0f, maxDelayBetweenTentaclesAttacks));
            }
            yield return new WaitForSeconds(Random.Range(minBossWavesDelay, maxBossWavesDelay));
        }
    }
    private void OnTentacleDefeated()
    {
        currentNumberOfTentacles++;
        if (currentNumberOfTentacles >= numberOfTentaclesToDefeatBoss && !bossDefeated)
        {
            EndBattle();
        }
    }
}
