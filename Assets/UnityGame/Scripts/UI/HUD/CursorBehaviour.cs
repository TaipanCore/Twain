using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : UICursorFollower
{
    private Image darkSideSpikesRecharge;
    private Image equilibriumAttackRecharge;
    private Image equilibriumShootRecharge;

    protected override void Start()
    {
        base.Start();
        G.characters.CharacterChange += ChangeCursor;
        G.HUD.StateChange += SetCursorVisibility;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        darkSideSpikesRecharge = transform.Find("DarkSideSpikesRecharge/Fill").GetComponent<Image>();
        equilibriumAttackRecharge = transform.Find("EquilibriumAttackRecharge/Fill").GetComponent<Image>();
        equilibriumShootRecharge = transform.Find("EquilibriumShootRecharge/Fill").GetComponent<Image>();
    }
    private void OnDestroy()
    {
        G.characters.CharacterChange -= ChangeCursor;
    }

    public void SetSpikesRecharge(float value)
    {
        darkSideSpikesRecharge.fillAmount = value;
    }
    public void SetAttackRecharge(float value)
    {
        equilibriumAttackRecharge.fillAmount = value;
    }
    public void SetShootRecharge(float value)
    {
        equilibriumShootRecharge.fillAmount = value;
    }

    private void SetCursorVisibility(HUD.State state)
    {
        switch (state)
        {
            case HUD.State.Game:
                Cursor.visible = false;
                break;
            case HUD.State.GameOver:
            case HUD.State.GameComplete:
            case HUD.State.Pause:
            case HUD.State.Map:
                Cursor.visible = true;
                break;
        }
    }
    
    private void ChangeCursor(GameObject newCharacter)
    {
        switch (newCharacter)
        {
            case { } character when character == G.characters.lightSide:
                darkSideSpikesRecharge.enabled = false;
                equilibriumAttackRecharge.enabled = false;
                equilibriumShootRecharge.enabled = false;
                break;
            case { } character when character == G.characters.darkSide:
                darkSideSpikesRecharge.enabled = true;
                equilibriumAttackRecharge.enabled = false;
                equilibriumShootRecharge.enabled = false;
                break;
            case { } character when character == G.characters.equilibrium:
                darkSideSpikesRecharge.enabled = false;
                equilibriumAttackRecharge.enabled = true;
                equilibriumShootRecharge.enabled = true;
                break;
        }
    }
}
