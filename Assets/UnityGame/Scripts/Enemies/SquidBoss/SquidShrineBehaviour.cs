using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using TentacleData = TentacleBehaviour.TentacleData;

public class SquidShrineBehaviour : MonoBehaviour, ISaveLoadObject
{
    [SerializeField] private SquidBossBehaviour squidBossBehaviour;
    [SerializeField] private CapsuleCollider2D activationCollider;
    
    private Transform fireflyPoint;
    private Transform firefly;
    private Vector3 fireflyOldLocalPosition;
    private SquidBossBehaviour.State bossState;
    private AudioSource bossMusic;

    private void Awake()
    {
        RegisterInSaveLoadSystem();
        
        fireflyPoint = transform.Find("FireflyPoint");
    }
    private void Start()
    {
        squidBossBehaviour.StateChanged += OnStateChanged;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (G.input.interactiveBtnDown)
        {
            if (other.TryGetComponent(out LightSideBehaviour lightSideBehaviour))
            {
                if (bossState == SquidBossBehaviour.State.Sleep)
                {
                    firefly = lightSideBehaviour.TakeFirefly(fireflyPoint);
                    squidBossBehaviour.StartBattle();
                    bossMusic = G.audio.PlayMusic(G.music.squidBossMusic, loop: false, fadeDuration: 1f);
                }
                else if (bossState == SquidBossBehaviour.State.Defeated || bossState == SquidBossBehaviour.State.Dead)
                {
                    lightSideBehaviour.ReturnFirefly();
                    firefly = null;
                    activationCollider.enabled = false;
                }
            }
        }
    }

    private void OnStateChanged(SquidBossBehaviour.State newState)
    {
        bossState = newState;
        if (newState == SquidBossBehaviour.State.Dead)
            squidBossBehaviour.StateChanged -= OnStateChanged;
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        bool hasFirefly = firefly && firefly.transform.parent == fireflyPoint;
        TentacleData[] tentaclesData = Array.Empty<TentacleData>();
        if (bossState == SquidBossBehaviour.State.Battle)
            tentaclesData = squidBossBehaviour.PackTentaclesData();
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            hasFirefly,
            bossState,
            squidBossBehaviour.currentNumberOfTentacles,
            tentaclesData,
            bossMusic ? bossMusic.time : 0f
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - hasFirefly
        if(bool.TryParse(dataToUnpack.data[0].ToString(), out var parsedHasFirefly))
            if (parsedHasFirefly)
                firefly = G.characters.lightSide.GetComponent<LightSideBehaviour>().TakeFirefly(fireflyPoint, 0f);
        //data[1] - bossState
        if (Enum.TryParse(dataToUnpack.data[1].ToString(), out SquidBossBehaviour.State parsedBossState))
        {
            bossState = parsedBossState;
            switch (bossState)
            {
                case SquidBossBehaviour.State.Battle:
                    break;
                case SquidBossBehaviour.State.Defeated:
                    squidBossBehaviour.SpawnEyes(false);
                    squidBossBehaviour.EndBattle(true);
                    break;
                case SquidBossBehaviour.State.Dead:
                    Destroy(squidBossBehaviour.gameObject);
                    break;
            }
        }

        if (bossState == SquidBossBehaviour.State.Battle)
        {
            //data[2] - currentNumberOfTentacles
            if(int.TryParse(dataToUnpack.data[2].ToString(), out var parsedCurrentNumberOfTentacles))
                squidBossBehaviour.currentNumberOfTentacles = parsedCurrentNumberOfTentacles;
            //data[3] - tentaclesData
            TentacleData[] tentaclesData = ((JArray)dataToUnpack.data[3]).ToObject<TentacleData[]>();
            squidBossBehaviour.UnpackTentaclesData(tentaclesData);
            squidBossBehaviour.StartBattle(false);
            //data[4] - bossMusicTime
            if(float.TryParse(dataToUnpack.data[4].ToString(), out var parsedBossMusicTime))
                bossMusic = G.audio.PlayMusic(G.music.squidBossMusic, loop: false, time: parsedBossMusicTime, fadeDuration: 1f);
        }
    }
}
