using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharactersManager : MonoBehaviour, ISaveLoadObject
{
    public GameObject equilibrium;
    public GameObject lightSide;
    public GameObject darkSide;
    
    [SerializeField] private float uniteDistance;
    [SerializeField] private Transform spawnPoint;

    [HideInInspector] public GameObject currentCharacter;
    [HideInInspector] public bool isUnited;
    [HideInInspector] public bool hasEquilibriumCharge;
    
    public event Action<GameObject> CharacterChange;
    public event Action PlayerDied;

    private void Awake()
    {
        G.characters = this;
        RegisterInSaveLoadSystem();
    }
    
    private void Update()
    {
        if (G.input.sidesChangeBtnDown && !isUnited)
        {
            ChangeSide();
        }
        if (G.input.uniteBtnDown && hasEquilibriumCharge && Utils.IsInRange(G.characters.lightSide.transform.position, G.characters.darkSide.transform.position, uniteDistance))
        {
            Unite();
        }
    }

    public void Unite()
    {
        hasEquilibriumCharge = false;
        lightSide.SetActive(false);
        darkSide.SetActive(false);
        equilibrium.SetActive(true);
        if (currentCharacter)
            equilibrium.transform.position = lightSide.transform.position;
        currentCharacter = equilibrium;
        isUnited = true;
        CharacterChange?.Invoke(currentCharacter);
    }
    public void Separate()
    {
        equilibrium.SetActive(false);
        lightSide.SetActive(true);
        darkSide.SetActive(true);
        if (currentCharacter)
        {
            lightSide.transform.position = currentCharacter.transform.position;
            darkSide.transform.position = currentCharacter.transform.position + (Vector3)Random.insideUnitCircle;
        }       
        currentCharacter = lightSide;
        CharacterChange?.Invoke(currentCharacter);
        isUnited = false;
    }
    private void ChangeSide()
    {
        if (currentCharacter == lightSide)
            currentCharacter = darkSide;
        else
            currentCharacter = lightSide;
        CharacterChange?.Invoke(currentCharacter);
    }

    public void GameOver()
    {
        G.audio.StopMusic();
        PlayerDied?.Invoke();
        G.gameOver.EndGame();
        GameObject secondCharacter = currentCharacter == lightSide ? darkSide : lightSide;
        currentCharacter.transform.position = spawnPoint.position;
        secondCharacter.transform.position = spawnPoint.position + (Vector3)Random.insideUnitCircle;
        LightSideBehaviour lightSideBehaviour = lightSide.GetComponent<LightSideBehaviour>();
        lightSideBehaviour.RestoreHealth();
        lightSideBehaviour.ReturnFirefly();
        lightSide.GetComponent<DarknessDeath>().GiveDarknessInvulnerability(3f);
        darkSide.GetComponent<DarknessDeath>().GiveDarknessInvulnerability(3f);
        G.mainCamera.transform.parent.position = currentCharacter.transform.position;
    }
    
    public String objectId => GetComponent<ObjectId>().id;
    public void RegisterInSaveLoadSystem() => G.gameSaveLoad.Register(this);
    public ObjectSaveLoadData PackData()
    {
        return new ObjectSaveLoadData(objectId, new System.Object[]
        {
            isUnited,
            currentCharacter.name,
            hasEquilibriumCharge
        });
    }
    public void UnpackData(ObjectSaveLoadData dataToUnpack)
    {
        //data[0] - isUnited
        if (bool.TryParse(dataToUnpack.data[0].ToString(), out var parsedIsUnited))
        {
            if (!isUnited && parsedIsUnited)
                Unite();
            else if (isUnited && !parsedIsUnited)
                Separate();
        }
        //data[1] - currentCharacter
        String currentCharacterName = dataToUnpack.data[1].ToString();
        switch (currentCharacterName)
        {
            case var characterName when characterName == lightSide.name:
                currentCharacter = lightSide;
                break;
            case var characterName when characterName == darkSide.name:
                currentCharacter = darkSide;
                break;
            case var characterName when characterName == equilibrium.name:
                currentCharacter = equilibrium;
                break;
        }
        CharacterChange?.Invoke(currentCharacter);
        //data[2] - hasEquilibriumCharge
        if (bool.TryParse(dataToUnpack.data[2].ToString(), out var parsedHasEquilibriumCharge))
            hasEquilibriumCharge = parsedHasEquilibriumCharge;
        if (hasEquilibriumCharge)
            G.HUD.equilibriumCharge.transform.Find("Background").GetComponent<Image>().DOFade(1f, 0f);
    }
}
