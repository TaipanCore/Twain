using UnityEngine;
using UnityEngine.EventSystems;

public class UIBtnSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip onPointerEnter;
    [SerializeField] private AudioClip onPointerClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        G.audio.PlaySoundEffect(onPointerEnter);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        G.audio.PlaySoundEffect(onPointerClick);
    }
}
