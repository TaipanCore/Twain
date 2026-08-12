using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ButtonsHints : MonoBehaviour
{
    public enum BtnKey
    {
        E,
        M
    }
    
    private Dictionary<BtnKey, HintInfo> btnKeysAndHints = new ();
    private Dictionary<HintInfo, Tween> activeHintsFade = new ();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out HintInfo hint))
                btnKeysAndHints.Add(hint.key, hint);
        }
    }
    
    public void ShowHint(BtnKey key)
    {
        if (btnKeysAndHints.ContainsKey(key) && btnKeysAndHints[key])
        {
            btnKeysAndHints[key].gameObject.SetActive(true);
            StartFade(btnKeysAndHints[key]);
        }
    }

    public void HideHint(BtnKey key)
    {
        if (btnKeysAndHints.ContainsKey(key) && btnKeysAndHints[key])
        {
            btnKeysAndHints[key].gameObject.SetActive(false);
            StopFade(btnKeysAndHints[key]);
        }
    }

    private void StartFade(HintInfo hint)
    {
        foreach (Tween fadeTween in activeHintsFade.Values)
            fadeTween.Restart();
        activeHintsFade.TryAdd(hint, hint.text.DOFade(1f, 0.5f).From(0.5f).SetLoops(-1, LoopType.Yoyo));
    }

    private void StopFade(HintInfo hint)
    {
        if (activeHintsFade.ContainsKey(hint))
        {
            Tween hintTween = activeHintsFade[hint];
            hintTween.Kill(true);
            activeHintsFade.Remove(hint);
        }
    }
}
