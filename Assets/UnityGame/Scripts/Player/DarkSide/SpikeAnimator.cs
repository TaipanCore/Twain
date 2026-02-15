using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField, Min(0)] private int framerate;

    private WaitForSeconds framerateDelay;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        framerateDelay = new WaitForSeconds(1.0f / framerate);
        StartCoroutine(PlaySpawn());
    }
    private IEnumerator PlaySpawn()
    {
        foreach (Sprite sprite in animationSprites)
        {
            spriteRenderer.sprite = sprite;
            yield return framerateDelay;
        }
    }
}