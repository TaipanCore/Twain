using UnityEngine;

public class SpriteOrderInLayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private Transform playerTransform;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer lightSideSpriteRenderer;
    private SpriteRenderer darkSideSpriteRenderer;
    private SpriteRenderer equilibriumSpriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightSideSpriteRenderer = GameManager.lightSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        darkSideSpriteRenderer = GameManager.darkSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        equilibriumSpriteRenderer = GameManager.equilibrium.transform.Find("Appearance").GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        playerTransform = GameManager.currentCharacter.transform;
        FindPlayerSpriteRenderer();
        CheckSpriteOrderInLayer();
    }
    private void FindPlayerSpriteRenderer()
    {
        if (GameManager.currentCharacter == GameManager.lightSide)
            playerSpriteRenderer = lightSideSpriteRenderer;
        else if (GameManager.currentCharacter == GameManager.darkSide)
            playerSpriteRenderer = darkSideSpriteRenderer;
        else if (GameManager.currentCharacter == GameManager.equilibrium)
            playerSpriteRenderer = equilibriumSpriteRenderer;
    }
    private void CheckSpriteOrderInLayer()
    {
        if (playerTransform.position.y > transform.position.y)
        {
            spriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
        }
        else
        {
            spriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
        }
    }
}
