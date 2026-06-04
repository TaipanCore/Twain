using System;
using DG.Tweening;
using UnityEngine;

public class ShardSlot : MonoBehaviour
{
    [SerializeField] private GameObject requiredShard;
    
    public event Action OnGetShard;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && InputManager.interactiveBtnDown && GameManager.HUD.inventory.items.ContainsKey(requiredShard))
        {
            if (!requiredShard.activeInHierarchy)
                requiredShard.SetActive(true);
            GameManager.HUD.inventory.RemoveItem(requiredShard);
            SpriteRenderer spriteRenderer = requiredShard.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "InteractiveObjects";
            spriteRenderer.sortingOrder = 2;
            requiredShard.transform.DOMove(transform.position, 0.5f).OnComplete(() =>
            {
                requiredShard.GetComponent<ParticleSystem>().Stop();
                requiredShard.GetComponent<SimpleAnimator>().Restart();
                OnGetShard?.Invoke();
            });
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
