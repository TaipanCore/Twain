using System.Collections;
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
    private void Start()
    {
        gameTitleImage = GetComponent<Image>();
        blockGameObject = transform.GetChild(0).gameObject;
        blockImage = blockGameObject.GetComponent<Image>();
        betweenBlockDropsWait = new WaitForSeconds(blockDropTime + delayBetweenBlockDrops);
        StartCoroutine(BlockDroppingCoroutine());
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
        Destroy(blockGameObject);
    }
    private void DropBlock(int stepIndex)
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .AppendCallback(() => blockImage.sprite = blocks[stepIndex])
            .Join(blockImage.DOFade(1f, blockDropTime)
                .From(0f))
            .Join(blockGameObject.transform.DOMoveY(blockGameObject.transform.position.y, blockDropTime)
                .From(blockGameObject.transform.position.y + blockDropOffset)
                .OnComplete(() => gameTitleImage.sprite = gameTitleStepsSprites[stepIndex]))
            .Join(DOVirtual.DelayedCall(blockDropTime * 0.9f,() => G.audio.PlaySoundEffect(dropBlockSound)));
    }
}
