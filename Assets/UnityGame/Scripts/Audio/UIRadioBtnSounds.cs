using UnityEngine;
using UnityEngine.EventSystems;

public class UIRadioBtnSounds : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip onPointerClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        G.audio.PlaySoundEffect(onPointerClick);
    }
}
