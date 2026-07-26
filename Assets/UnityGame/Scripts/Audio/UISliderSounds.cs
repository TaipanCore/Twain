using UnityEngine;
using UnityEngine.UI;

public class UISliderSounds : MonoBehaviour
{
    [SerializeField] private AudioClip onValueChanged;
    
    [SerializeField] private float pitchMin;
    [SerializeField] private float pitchMax;

    private Slider slider;
    private int previousValue;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        previousValue = Mathf.RoundToInt(slider.value);
    }
    
    public void OnValueChanged(float newValue)
    {
        int currentValue = Mathf.RoundToInt(newValue);
        if (currentValue != previousValue)
        {
            previousValue = currentValue;
            G.audio.PlaySoundEffect(onValueChanged, pitchMin: pitchMin, pitchMax: pitchMax);
        }
    }
}
