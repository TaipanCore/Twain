using UnityEngine;
using UnityEngine.UI;

public class CursorBehaviour : MonoBehaviour
{
    private RectTransform cursorTransform;
    private Canvas parentCanvas;
    private RectTransform parentCanvasTransform;
    private Vector2 mousePos;

    private Image darkSideSpikesRecharge;
    private Image equilibriumAttackRecharge;
    private Image equilibriumShootRecharge;

    private void Start()
    {
        GameManager.OnCharacterChange += ChangeCursor;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        parentCanvas = transform.parent.GetComponent<Canvas>();
        parentCanvasTransform = parentCanvas.GetComponent<RectTransform>();
        cursorTransform = GetComponent<RectTransform>();
        darkSideSpikesRecharge = transform.Find("DarkSideSpikesRecharge/Fill").GetComponent<Image>();
        equilibriumAttackRecharge = transform.Find("EquilibriumAttackRecharge/Fill").GetComponent<Image>();
        equilibriumShootRecharge = transform.Find("EquilibriumShootRecharge/Fill").GetComponent<Image>();
    }
    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvasTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePos);
        cursorTransform.localPosition = mousePos;
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
    
    private void ChangeCursor(GameObject newCharacter)
    {
        switch (newCharacter)
        {
            case { } character when character == GameManager.lightSide:
                darkSideSpikesRecharge.enabled = false;
                equilibriumAttackRecharge.enabled = false;
                equilibriumShootRecharge.enabled = false;
                break;
            case { } character when character == GameManager.darkSide:
                darkSideSpikesRecharge.enabled = true;
                equilibriumAttackRecharge.enabled = false;
                equilibriumShootRecharge.enabled = false;
                break;
            case { } character when character == GameManager.equilibrium:
                darkSideSpikesRecharge.enabled = false;
                equilibriumAttackRecharge.enabled = true;
                equilibriumShootRecharge.enabled = true;
                break;
        }
    }
}
