using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquidBossBehaviour : MonoBehaviour
{
    public enum State
    {
        Sleep,
        Battle,
        Defeated,
        Dead
    }
    
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

    public event Action<State> StateChanged;
    [HideInInspector] public int currentNumberOfTentacles;
    
    private ParticleSystem soundWaveParticles;
    private SquidBossSounds squidBossSounds;
    private State currentState;

    private void Start()
    {
        eyeAppearTimer = new WaitForSeconds(eyeAppearDelay);
        soundWaveParticles = GetComponent<ParticleSystem>();
        squidBossSounds = GetComponent<SquidBossSounds>();
        reward.SetActive(false);
        G.characters.PlayerDied += OnPlayerDied;
        SetState(State.Sleep);
    }
    private void OnDestroy()
    {
        G.characters.PlayerDied -= OnPlayerDied;
    }

    public void SetState(State newState)
    {
        currentState = newState;
        StateChanged?.Invoke(newState);
    }
    public void StartBattle(bool playSpawnEyesAnimation = true)
    {
        SetState(State.Battle);
        SpawnEyes(playSpawnEyesAnimation);
    }
    public void EndBattle(bool isPlayerWon)
    {
        if (isPlayerWon)
        {
            SetState(State.Defeated);
            BossDying();
        }
        else
        {
            SetState(State.Sleep);
            RetreatAllTentacles();
            HideEyes();
            currentNumberOfTentacles = 0;
        }
    }

    private void RetreatAllTentacles()
    {
        foreach (TentacleBehaviour tentacle in tentacles)
        {
            tentacle.OnDefeated -= OnTentacleDefeated;
            tentacle.Retreat();
            DOVirtual.DelayedCall(timeToRetreat, () => tentacle.gameObject.SetActive(false), false);
        }
    }

    private void BossDying()
    {
        Sequence dyingSequence = DOTween.Sequence();
        dyingSequence
            .AppendCallback(RetreatAllTentacles)
            .AppendCallback(squidBossSounds.PlayDieSound)
            .Append(transform.DOPunchPosition(Vector3.up * 0.35f, 1f, 4))
            .AppendCallback(HideEyes)
            .AppendInterval(2f)
            .Append(DropReward())
            .AppendCallback(() => G.audio.PlayMusic(G.music.labyrinthMusic))
            .AppendCallback(() =>
            {
                SetState(State.Dead);
                Destroy(gameObject);
            });
        dyingSequence.Play();
    }

    private void HideEyes()
    {
        foreach (GameObject eye in eyes)
        {
            eye.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(() => eye.SetActive(false));
        }
    }
    private Tween DropReward()
    {
        reward.SetActive(true);
        reward.transform.position = transform.position;
        Vector2 jumpToPosition = transform.position + (Vector3)Random.insideUnitCircle.normalized * 2f;
        ParticleSystem trailParticles = reward.GetComponent<ParticleSystem>();
        return reward.transform.DOJump(jumpToPosition, 2f, 1, 0.6f).SetEase(Ease.Linear).OnComplete(() => trailParticles.Stop());
    }

    public void SpawnEyes(bool playAnimation)
    {
        if (playAnimation)
            StartCoroutine(SpawnEyesCoroutine());
        else
        {
            foreach (GameObject eye in eyes)
                eye.SetActive(true);
            StartCoroutine(TentaclesAttack());
        }
    }
    private IEnumerator SpawnEyesCoroutine()
    {
        G.input.canPlayerInput = false;
        for (int i = 0; i < eyes.Length; i++)
        {
            GameObject eye = eyes[i];
            squidBossSounds.PlayEyePopSound(eye.transform.position, 0.4f + 0.4f * i / eyes.Length);
            eye.SetActive(true);
            eye.GetComponent<SpriteRenderer>().DOFade(1f, 0f);
            eye.GetComponent<SimpleAnimator>().Restart();
            yield return eyeAppearTimer;
        }
        soundWaveParticles.Play();
        squidBossSounds.PlayAppearSound();
        yield return new WaitForSeconds(soundWaveParticles.main.duration);
        G.input.canPlayerInput = true;
        yield return TentaclesAttack();
    }
    private IEnumerator TentaclesAttack()
    {
        foreach (TentacleBehaviour tentacle in tentacles)
        {
            tentacle.gameObject.SetActive(true);
            tentacle.OnDefeated += OnTentacleDefeated;
        }
        while (currentState == State.Battle)
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
        squidBossSounds.PlayTentacleHitSound();
        currentNumberOfTentacles++;
        if (currentNumberOfTentacles >= numberOfTentaclesToDefeatBoss && currentState == State.Battle)
        {
            EndBattle(true);
        }
    }

    private void OnPlayerDied()
    {
        EndBattle(false);
    }

    public TentacleBehaviour.TentacleData[] PackTentaclesData()
    {
        TentacleBehaviour.TentacleData[] data = new TentacleBehaviour.TentacleData[tentacles.Length];
        for (int i = 0; i < data.Length; i++)
            data[i] = new TentacleBehaviour.TentacleData(
                tentacles[i].transform.position,
                tentacles[i].currentState,
                tentacles[i].GetStateElapsedTime()
            );
        return data;
    }

    public void UnpackTentaclesData(TentacleBehaviour.TentacleData[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            tentacles[i].transform.position = data[i].position;
            switch (data[i].state)
            {
                case TentacleBehaviour.State.Attack:
                    tentacles[i].timeToReachTarget = timeToReachTarget - data[i].stateElapsedTime;
                    tentacles[i].timeToRetreat = timeToRetreat;
                    tentacles[i].Attack();
                    break;
                case TentacleBehaviour.State.Retreat:
                    tentacles[i].timeToReachTarget = timeToReachTarget;
                    tentacles[i].timeToRetreat = timeToRetreat - data[i].stateElapsedTime;
                    tentacles[i].Retreat();
                    break;
            }
        }
    }
}
