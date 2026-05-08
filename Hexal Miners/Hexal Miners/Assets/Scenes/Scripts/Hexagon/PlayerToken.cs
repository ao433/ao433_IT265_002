using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToken : Entity
{
    [Header("Player Info")]
    public string tokenName;        
    public int currentX;
    public int currentY;
    public int playerNumber;
    public Color tokenColor;

    [Header("Movement")]
    public bool isMoving = false;

    private GameMapManager mapManager;

    protected override void Start()
    {
        base.Start();

      
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        mapManager = FindFirstObjectByType<GameMapManager>();
    }

    public void Initialize(string name, int startX, int startY, int playerNum, Color color)
    {
        entityName = name;
        tokenName = name;
        currentX = startX;
        currentY = startY;
        playerNumber = playerNum;
        tokenColor = color;

        // Start with 1 defense, 0 hit power
        maxDefense = 1;
        currentDefense = maxDefense;
        hitPower = 0;  // Start at 0 - cards provide temporary boosts

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            originalColor = color;
        }

        UpdatePosition();
    }

    public void UpdatePosition()
    {
      
        if (mapManager == null)
        {
            mapManager = FindFirstObjectByType<GameMapManager>();
        }

        if (mapManager != null)
        {
            Vector2 worldPos = new Vector2(
                mapManager.tileIncrementX * currentX,
                mapManager.tileIncrementY * currentY
            );
            transform.position = worldPos;
            Debug.Log($"{tokenName} position updated to ({currentX}, {currentY})");
        }
    }

    public void MoveTo(int newX, int newY)
    {
        currentX = newX;
        currentY = newY;
        UpdatePosition();
        Debug.Log($"{tokenName} moved to position ({currentX}, {currentY})");
    }

    protected override void OnDefeated()
    {
        base.OnDefeated();
        Stun(); // Player gets stunned when defense hits 0
        Debug.Log($"{tokenName} was defeated and is now stunned!");
    }

    public override void Stun()
    {
        base.Stun();
        Debug.Log($"{tokenName} is stunned for one turn!");
    }

    public override void RecoverFromStun()
    {
        base.RecoverFromStun();
        Debug.Log($"{tokenName} recovered from stun! Defense restored to {currentDefense}/{maxDefense}");
    }

    public void AttackEntity(Entity target)
    {
        if (isStunned || isDestroyed)
        {
            Debug.Log($"{tokenName} cannot attack - stunned or destroyed!");
            return;
        }

        target.TakeHit(hitPower);
        Debug.Log($"{tokenName} attacked {target.entityName} for {hitPower} damage!");
    }

    public void AttackTile(GameMapTile tile)
    {
        if (isStunned || isDestroyed)
        {
            Debug.Log($"{tokenName} cannot attack tile - stunned or destroyed!");
            return;
        }

        if (mapManager != null)
        {
            mapManager.DamageTile(tile, hitPower);
            Debug.Log($"{tokenName} attacked tile at ({tile.x}, {tile.y}) for {hitPower} damage!");
        }
    }


    public void AddDefense(int amount)
    {
        maxDefense += amount;
        currentDefense += amount;
        Debug.Log($"{tokenName} gained +{amount} Defense! Now at {currentDefense}/{maxDefense}");
    }

    public void AddHitPower(int amount)
    {
        hitPower += amount;
        Debug.Log($"{tokenName} gained +{amount} Hit Power! Now at {hitPower}");
    }

    public void Heal(int amount)
    {
        if (isDestroyed) return;

        currentDefense = Mathf.Min(currentDefense + amount, maxDefense);
        Debug.Log($"{tokenName} healed {amount} Defense! Now at {currentDefense}/{maxDefense}");
    }


    public bool CanAct()
    {
        return !isStunned && !isDestroyed;
    }

    //current stats for UI display
    public int GetCurrentDefense() => currentDefense;
    public int GetMaxDefense() => maxDefense;
    public int GetHitPower() => hitPower;
}