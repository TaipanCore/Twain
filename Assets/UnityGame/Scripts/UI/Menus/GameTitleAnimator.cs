using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameTitleAnimator : MonoBehaviour
{
    [SerializeField] private AudioClip dropBlockSound;
    [SerializeField] private float blockDropOffset;
    [SerializeField] private float blockDropTime;
    [SerializeField] private float delayBetweenBlockDrops;
    private WaitForSeconds betweenBlockDropsWait;
    [SerializeField] private Sprite gameTitleEmpty;
    [SerializeField] private Sprite[] blocks;
    [SerializeField] private Sprite[] gameTitleStepsSprites;
    
    private Image gameTitleImage;
    private Image blockImage;
    private GameObject blockGameObject;
    private Sequence blockDropSequence;
    private void Start()
    {
        gameTitleImage = GetComponent<Image>();
        blockGameObject = transform.GetChild(0).gameObject;
        blockImage = blockGameObject.GetComponent<Image>();
        betweenBlockDropsWait = new WaitForSeconds(blockDropTime + delayBetweenBlockDrops);
        StartCoroutine(BlockDroppingCoroutine());
    }

    private void OnDisable()
    {
        EndAnimation();
    }

    private IEnumerator BlockDroppingCoroutine()
    {
        gameTitleImage.sprite = gameTitleEmpty;
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < blocks.Length; i++)
        {
            DropBlock(i);
            yield return betweenBlockDropsWait;
        }
        yield return new WaitForSeconds(0.25f);
        EndAnimation();
    }
    private void DropBlock(int stepIndex)
    {
        blockDropSequence?.Kill();
        blockDropSequence = DOTween.Sequence();
        blockDropSequence
            .AppendCallback(() => blockImage.sprite = blocks[stepIndex])
            .Join(blockImage.DOFade(1f, blockDropTime)
                .From(0f))
            .Join(blockGameObject.transform.DOMoveY(blockGameObject.transform.position.y, blockDropTime)
                .From(blockGameObject.transform.position.y + blockDropOffset)
                .OnComplete(() => gameTitleImage.sprite = gameTitleStepsSprites[stepIndex]))
            .Join(DOVirtual.DelayedCall(blockDropTime * 0.9f,() => G.audio.PlaySoundEffect(dropBlockSound)));
    }

    private void EndAnimation()
    {
        blockDropSequence?.Kill();
        gameTitleImage.sprite = gameTitleStepsSprites.Last();
        Destroy(blockGameObject);
    }
}
