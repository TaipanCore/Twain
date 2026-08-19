using System;
using DG.Tweening;
using UnityEngine;

public class ShardSlot : MonoBehaviour
{
    [SerializeField] private GameObject requiredShard;
    
    public event Action OnGetShard;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && G.input.interactiveBtnDown && G.HUD.inventory.items.ContainsKey(requiredShard))
        {
            G.HUD.inventory.RemoveItem(requiredShard);
            requiredShard.GetComponent<ShardBehaviour>().owner = ShardBehaviour.Owner.FinalGates;
            MoveShardToSlot();
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void MoveShardToSlot()
    {
        if (!requiredShard.activeInHierarchy)
            requiredShard.SetActive(true);
        SpriteRenderer spriteRenderer = requiredShard.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "InteractiveObjects";
        spriteRenderer.sortingOrder = 2;
        if (G.HUD.inventory.items.TryGetValue(requiredShard, out var inventoryShard))
            requiredShard.transform.position = inventoryShard.transform.position;
        requiredShard.transform.DOMove(transform.position, 0.5f).OnComplete(() =>
        {
            requiredShard.GetComponent<ParticleSystem>().Stop();
            requiredShard.GetComponent<SimpleAnimator>().Restart();
            requiredShard.GetComponent<ShardSounds>().PlayMoveToSlotSound();
            OnGetShard?.Invoke();
        });
    }
}
