using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeathBar : MonoBehaviour
{
    [SerializeField] private Sprite lightSideHealthBar;
    [SerializeField] private Sprite darkSideHealthBar;
    [SerializeField] private Sprite equilibriumHealthBar;
    
    private Image healthBar;
    private Image healthBarFill;
    private Slider healthBarSlider;

    private LightSideBehaviour lightSideBehaviour;

    private void Start()
    {
        GameManager.OnCharacterChange += ChangeHealthBar;
        healthBar = transform.Find("Background").GetComponent<Image>();
        healthBarFill = transform.Find("Fill Area/Fill").GetComponent<Image>();
        healthBarSlider = GetComponent<Slider>();
        lightSideBehaviour = GameManager.lightSide.GetComponent<LightSideBehaviour>();
    }

    public void SetValue(float value)
    {
        healthBarSlider.value = value;
    }
    private void ChangeHealthBar(GameObject newCharacter)
    {
        switch (newCharacter)
        {
            case { } character when character == GameManager.lightSide:
                healthBarSlider.value = lightSideBehaviour.hitpoints;
                healthBar.sprite = lightSideHealthBar;
                healthBarFill.color = Color.red;
                break;
            case { } character when character == GameManager.darkSide:
                healthBarSlider.value = healthBarSlider.maxValue;
                healthBar.sprite = darkSideHealthBar;
                healthBarFill.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case { } character when character == GameManager.equilibrium:
                healthBarSlider.value = healthBarSlider.maxValue;
                healthBar.sprite = equilibriumHealthBar;
                healthBarFill.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                break;
        }
    }
}
