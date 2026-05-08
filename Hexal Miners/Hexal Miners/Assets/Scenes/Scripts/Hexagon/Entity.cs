using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Stats")]
    public string entityName;
    public int maxDefense = 1;
    public int currentDefense;
    public int hitPower = 1;

    [Header("State")]
    public bool isStunned = false;
    public bool isDestroyed = false;

    public SpriteRenderer spriteRenderer;
    protected Color originalColor;

    protected virtual void Start()
    {
        currentDefense = maxDefense;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public virtual void TakeHit(int damage)
    {
        if (isDestroyed) return;

        currentDefense -= damage;
        Debug.Log($"{entityName} took {damage} damage! Defense: {currentDefense}/{maxDefense}");

        if (currentDefense <= 0)
        {
            OnDefeated();
        }
        else
        {
            // Visual feedback for taking damage
            StartCoroutine(DamageFlash());
        }
    }

    protected virtual void OnDefeated()
    {
        currentDefense = 0;
        isDestroyed = true;
        Debug.Log($"{entityName} was defeated!");
    }

    public virtual void Stun()
    {
        if (isDestroyed) return;

        isStunned = true;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Darkened

        Debug.Log($"{entityName} is stunned!");
    }

    public virtual void RecoverFromStun()
    {
        isStunned = false;
        currentDefense = maxDefense;
        isDestroyed = false; // Reset destroyed flag

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        Debug.Log($"{entityName} recovered from stun! Defense restored to {currentDefense}");
    }

    protected IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            Color flashColor = Color.red;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.15f);

            if (!isStunned && !isDestroyed)
                spriteRenderer.color = originalColor;
            else if (isStunned)
                spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }
    }

    public void SetStats(int defense, int hit)
    {
        maxDefense = defense;
        currentDefense = defense;
        hitPower = hit;
    }
    public bool IsAlive() => !isDestroyed && currentDefense > 0;
    public bool CanAct() => !isStunned && !isDestroyed;
}