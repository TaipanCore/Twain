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
        lightSideSpriteRenderer = G.characters.lightSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        darkSideSpriteRenderer = G.characters.darkSide.transform.Find("Appearance").GetComponent<SpriteRenderer>();
        equilibriumSpriteRenderer = G.characters.equilibrium.transform.Find("Appearance").GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        playerTransform = G.characters.currentCharacter.transform;
        FindPlayerSpriteRenderer();
        CheckSpriteOrderInLayer();
    }
    private void FindPlayerSpriteRenderer()
    {
        if (G.characters.currentCharacter == G.characters.lightSide)
            playerSpriteRenderer = lightSideSpriteRenderer;
        else if (G.characters.currentCharacter == G.characters.darkSide)
            playerSpriteRenderer = darkSideSpriteRenderer;
        else if (G.characters.currentCharacter == G.characters.equilibrium)
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
