using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;

    // Colors for highlighting
    public Color normalColor;
    public Color highlightColor = Color.yellow;

    // Store original color and tile data
    private Color originalColor;
    public GameMapTile tileData; 

    
    public System.Action<GameMapTile> OnTileClicked;

   
    private bool isHighlighted = false;

  
    private Entity tileEntity;

    void Start()
    {
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            normalColor = originalColor;
            spriteRenderer.sortingOrder = 0; // Tiles at layer 0
        }

        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                collider.size = spriteRenderer.sprite.bounds.size;
            }
            Debug.Log("Collider added to tile");
        }

        
        tileEntity = gameObject.AddComponent<Entity>();
        if (tileData != null)
        {
            tileEntity.entityName = $"Tile ({tileData.x}, {tileData.y})";
            // Set defense based on tile type
            SetTileDefense(GetDefenseFromTileType(tileData.tileType));
        }
        else
        {
            tileEntity.entityName = $"Tile (Unknown)";
        }
    }

  
    private int GetDefenseFromTileType(int tileType)
    {
        switch (tileType)
        {
            case 0: // Bronze/Brown
                return 1;
            case 1: // Silver
                return 2;
            case 2: // Gold
                return 3;
            default:
                return 1;
        }
    }

    public void SetTileColor(Color color)
    {
        spriteRenderer.color = color;
        originalColor = color;
        normalColor = color;
    }

    public void Initialize(GameMapTile tile, Color color)
    {
        tileData = tile;
        SetTileColor(color);

   
        if (tileEntity != null && tile != null)
        {
            tileEntity.entityName = $"Tile ({tile.x}, {tile.y})";
            SetTileDefense(GetDefenseFromTileType(tile.tileType));
        }
    }


    public void SetHighlightColor(Color color)
    {
        if (spriteRenderer != null && IsTileAvailable()) 
        {
            spriteRenderer.color = color;
            isHighlighted = true;
        }
    }

    public void ResetToOriginalColor()
    {
        if (spriteRenderer != null && isHighlighted)
        {
            spriteRenderer.color = originalColor;
            isHighlighted = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsTileAvailable()) return; 

        Debug.Log($"Hovering over tile at ({tileData?.x}, {tileData?.y})");
        if (spriteRenderer != null && !isHighlighted)
        {
            spriteRenderer.color = highlightColor;
            spriteRenderer.sortingOrder = 20;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsTileAvailable()) return;

        Debug.Log($"Stopped hovering over tile at ({tileData?.x}, {tileData?.y})");
        if (spriteRenderer != null && !isHighlighted)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.sortingOrder = 10;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsTileAvailable())
        {
            Debug.Log($"Tile at ({tileData?.x}, {tileData?.y}) is destroyed and cannot be clicked!");
            return;
        }

        Debug.Log($"CLICKED on tile at ({tileData?.x}, {tileData?.y}) - Defense: {GetTileDefense()}");
        OnTileClicked?.Invoke(tileData);
        StartCoroutine(ClickFeedback());
    }

    private IEnumerator ClickFeedback()
    {
        if (spriteRenderer != null && IsTileAvailable())
        {
            Color flashColor = Color.white;
            Color previousColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = previousColor;
        }
    }



    public void SetTileDefense(int defense)
    {
        if (tileEntity != null)
        {
            tileEntity.maxDefense = defense;
            tileEntity.currentDefense = defense;
            Debug.Log($"Tile at ({tileData?.x}, {tileData?.y}) defense set to {defense}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (tileEntity != null)
        {
            int previousDefense = tileEntity.currentDefense;
            tileEntity.TakeHit(damage);

            // Visual feedback - darken based on damage
            if (spriteRenderer != null && tileData != null)
            {
                float healthPercent = (float)tileEntity.currentDefense / tileEntity.maxDefense;
                Color baseColor = originalColor;
                spriteRenderer.color = Color.Lerp(new Color(0.3f, 0.3f, 0.3f), baseColor, healthPercent);
            }

            if (tileEntity.isDestroyed)
            {
                // Mark tile as unavailable
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                spriteRenderer.color = new Color(0.15f, 0.15f, 0.15f, 0.5f); // Darkened destroyed tile

                // Update tileData
                if (tileData != null)
                {
                    tileData.isDestroyed = true;
                    tileData.currentDefense = 0;
                }

                Debug.Log($" Tile at ({tileData?.x}, {tileData?.y}) was DESTROYED!");
            }
            else
            {
                // Update tileData
                if (tileData != null)
                {
                    tileData.currentDefense = tileEntity.currentDefense;
                }
                Debug.Log($"Tile at ({tileData?.x}, {tileData?.y}) took {damage} damage! Defense: {tileEntity.currentDefense}/{tileEntity.maxDefense}");
            }
        }
    }

    public bool IsTileAvailable()
    {
        return tileEntity != null && !tileEntity.isDestroyed;
    }

    public int GetTileDefense()
    {
        return tileEntity != null ? tileEntity.currentDefense : 0;
    }

    public int GetTileMaxDefense()
    {
        return tileEntity != null ? tileEntity.maxDefense : 0;
    }

    public void RepairTile(int amount)
    {
        if (tileEntity != null && !tileEntity.isDestroyed)
        {
            tileEntity.currentDefense = Mathf.Min(tileEntity.currentDefense + amount, tileEntity.maxDefense);

            // Update visual
            if (spriteRenderer != null)
            {
                float healthPercent = (float)tileEntity.currentDefense / tileEntity.maxDefense;
                spriteRenderer.color = Color.Lerp(new Color(0.3f, 0.3f, 0.3f), originalColor, healthPercent);
            }

            // Update tileData
            if (tileData != null)
            {
                tileData.currentDefense = tileEntity.currentDefense;
            }
        }
    }
}