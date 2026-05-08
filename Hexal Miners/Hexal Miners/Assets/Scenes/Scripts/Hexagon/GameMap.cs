using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap
{
    public int gridWidth;
    public int gridHeight;
    public List<GameMapTile> allTiles;
    public Dictionary<string, GameMapTile> map;

    public int randomSeed;


    public GameMap(int width, int height)
    {
        this.gridWidth = width;
        this.gridHeight = height;
        allTiles = new List<GameMapTile>();
        map = new Dictionary<string, GameMapTile>();

   
        randomSeed = Random.Range(0, 10000);
        Random.InitState(randomSeed);
    }

    public void AddTile(float x, float y, int tileType)
    {
        string key = $"{x}, {y}";
        if (!map.ContainsKey(key))
        {
            GameMapTile newTile = new GameMapTile(x, y);
            newTile.tileType = tileType;
            allTiles.Add(newTile);
            map.Add(key, newTile);
        }
    }

    public void AutoGenerateMap()
    {
        // Clear existing tiles
        allTiles.Clear();
        map.Clear();

        // Generate rectangular grid
        int startX = -Mathf.FloorToInt(gridWidth / 2);
        int startY = -Mathf.FloorToInt(gridHeight / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int worldX = startX + x;
                int worldY = startY + y;

     
                int tileType = Random.Range(0, 3); 
                AddTile(worldX, worldY, tileType);
            }
        }

        Debug.Log($"Generated {allTiles.Count} random tiles in a {gridWidth}x{gridHeight} grid");
    }
}