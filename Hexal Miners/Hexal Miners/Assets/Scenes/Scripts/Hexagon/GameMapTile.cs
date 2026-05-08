using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapTile
{
    public float x, y;
    public int tileType;

    // Hit/Defense System
    public int defense;
    public int currentDefense;
    public bool isDestroyed;

    // WIN STATE
    public bool isWinTile = false;

    public GameMapTile(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetTileType(int tileType)
    {
        this.tileType = tileType;

        switch (tileType)
        {
            case 0: defense = 1; break;  // Brown
            case 1: defense = 2; break;  // Silver
            case 2: defense = 3; break;  // Gold
            default: defense = 1; break;
        }
        currentDefense = defense;
        isDestroyed = false;
    }

    public void TakeDamage(int damage)
    {
        currentDefense -= damage;
        if (currentDefense <= 0)
        {
            currentDefense = 0;
            isDestroyed = true;
        }
    }
}