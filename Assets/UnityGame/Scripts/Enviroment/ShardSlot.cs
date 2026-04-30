using System;
using DG.Tweening;
using UnityEngine;

public class ShardSlot : MonoBehaviour
{
    [SerializeField] private GameObject requiredShard;
    
    public event Action OnGetShard;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && InputManager.interactiveBtnDown && GameManager.inventory.Contains(requiredShard))
        {
            if (!requiredShard.activeInHierarchy)
                requiredShard.SetActive(true);
            GameManager.inventory.Remove(requiredShard);
            requiredShard.transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                SpriteRenderer spriteRenderer = requiredShard.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Ground";
                spriteRenderer.sortingOrder = 2;
                requiredShard.GetComponent<ParticleSystem>().Stop();
                requiredShard.GetComponent<SimpleAnimator>().Restart();
                OnGetShard?.Invoke();
            });
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
