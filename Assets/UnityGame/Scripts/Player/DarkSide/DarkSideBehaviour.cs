using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using SpikeData = SpikesSpawner.SpikeData;
using DarknessDeathData = DarknessDeath.DarknessDeathData;

public class DarkSideBehaviour : MonoBehaviour, ISaveLoadObject
{
    [Header("Spikes")]
    [SerializeField, Min(0)] private float spikesSpawnCooldown;
    private float spikesCooldownTimer;
    [SerializeField, Min(0)] private int numberOfSpikes;
    [SerializeField, Min(0)] private float spikesLifetime;
    [SerializeField] private GameObject spikesSpawner;
    [SerializeField] private float eyesSpawnMaxRadius;

    private Transform objectTransform;
    private Dictionary<Vector3, Tween> activeSpikes = new();
    
    private void Awake()
    {
        RegisterInSaveLoadSystem();
    }
    private void Start()
    {
        objectTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (spikesCooldownTimer > 0f && gameObject.activeInHierarchy)
        {
            spikesCooldownTimer -= Time.deltaTime;
            G.HUD.mouseCursor.SetSpikesRecharge((spikesSpawnCooldown - spikesCooldownTimer) / spikesSpawnCooldown);
        }
        if (G.input.leftMouseBtnDown && spikesCooldownTimer <= 0f && G.characters.currentCharacter == gameObject)
        {
            SpawnSpikes();
        }
    }

    private void SpawnSpikes()
    {
        GameObject spawnerObj = Instantiate(spikesSpawner, objectTransform.position, Quaternion.identity);
        SpikesSpawner spawnerScript = spawnerObj.GetComponent<SpikesSpawner>();
        spawnerScript.Initialize(numberOfSpikes, spikesLifetime, activeSpikes);
        StartCoroutine(spawnerScript.SpawnSpikes());
        G.HUD.mouseCursor.SetSpikesRecharge(0f);
        spikesCooldownTimer = spikesSpawnCooldown;
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        List<SpikeData> spikesData = new();
        foreach (KeyValuePair<Vector3, Tween> pair in activeSpikes)
            spikesData.Add(new SpikeData(pair.Key, spikesLifetime - pair.Value.Elapsed(false)));
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            transform.position,
            spikesCooldownTimer,
            spikesData.ToArray(),
            GetComponent<DarknessDeath>().PackDarknessDeathData()
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - position
        transform.position = ((JObject)dataToUnpack.data[0]).ToObject<Vector3>();
        //data[1] - spikesCooldownTimer
        if (float.TryParse(dataToUnpack.data[1].ToString(), out var parsedSpikesCooldownTimer))
            spikesCooldownTimer = parsedSpikesCooldownTimer;
        //data[2] - spikesData
        SpikeData[] serializedSpikesData = ((JArray)dataToUnpack.data[2]).ToObject<SpikeData[]>();
        GameObject spawnerObj = Instantiate(spikesSpawner, objectTransform.position, Quaternion.identity);
        SpikesSpawner spawnerScript = spawnerObj.GetComponent<SpikesSpawner>();
        spawnerScript.Initialize(numberOfSpikes, spikesLifetime, activeSpikes);
        spawnerScript.RestoreSpawnedSpikes(serializedSpikesData);
        //data[3] - darknessDeathData
        DarknessDeathData serializedDarknessDeathData = ((JObject)dataToUnpack.data[3]).ToObject<DarknessDeathData>();
        GetComponent<DarknessDeath>().UnpackDarknessDeathData(serializedDarknessDeathData);
    }
}
