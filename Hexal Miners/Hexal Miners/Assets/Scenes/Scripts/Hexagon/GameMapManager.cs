using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 

public class GameMapManager : MonoBehaviour
{
    public float tileIncrementX = 0.88f;
    public float tileIncrementY = 1.0f;
    public Color[] tileColor;
    public GameObject tileToGenerate;
    public GameMap currentMap;

    public int gridWidth = 10;
    public int gridHeight = 10;

    [Header("Camera Settings")]
    public float cameraPadding = 1.5f;

    private List<TileManager> allTiles = new List<TileManager>();

    [Range(1.2f, 2.5f)]
    public float highlightIntensity = 1.5f;

    public System.Action<GameMapTile> OnTileClickedEvent;

    [Header("Win State")]
    public GameObject winPanel;
    public TMP_Text winText;
    private GameMapTile winTile;
    private bool winTileRevealed = false;

    public void Start()
    {
        Debug.Log($"🚀 GameMapManager Start - tileIncrementX: {tileIncrementX}, tileIncrementY: {tileIncrementY}");

        CheckMouseInteractionComponents();

        currentMap = new GameMap(gridWidth, gridHeight);
        Debug.Log($"Creating {gridWidth}x{gridHeight} grid");

        currentMap.AutoGenerateMap();
        Debug.Log($"Map size: {currentMap.allTiles.Count}");

        //WinPanel starts hidden
        if (winPanel != null)
            winPanel.SetActive(false);

        showMap();
        SelectRandomWinTile(); 
    }



    void SelectRandomWinTile()
    {
        if (currentMap.allTiles.Count == 0) return;

        int randomIndex = Random.Range(0, currentMap.allTiles.Count);
        winTile = currentMap.allTiles[randomIndex];
        winTile.isWinTile = true;

        Debug.Log($"🏆 Win tile set at ({winTile.x}, {winTile.y})");
    }

    void Update()
    {
      
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ToggleWinTileReveal();
        }
    }

    void ToggleWinTileReveal()
    {
        winTileRevealed = !winTileRevealed;

        TileManager[] tiles = FindObjectsByType<TileManager>(FindObjectsSortMode.None);
        foreach (TileManager tileManager in tiles)
        {
            if (tileManager.tileData == winTile)
            {
                if (winTileRevealed)
                {
                    tileManager.SetTileColor(Color.black);
                    Debug.Log($"Win tile revealed at ({winTile.x}, {winTile.y})");
                }
                else
                {
                    tileManager.SetTileColor(tileColor[winTile.tileType]);
                    Debug.Log($" Win tile hidden");
                }
                break;
            }
        }
    }

    public void CheckWinCondition(GameMapTile tile)
    {
        if (tile == winTile && tile.isDestroyed)
        {
            Debug.Log($" WINNER! Tile at ({tile.x}, {tile.y}) was the win tile!");
            ShowWinScreen();
        }
    }

    void ShowWinScreen()
    {
        PlayerToken currentPlayer = FindFirstObjectByType<UnitManager>()?.GetCurrentPlayer();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (winText != null && currentPlayer != null)
            {
                winText.text = $"{currentPlayer.tokenName} WINS!!!";
            }
        }

        Time.timeScale = 0f;

        Debug.Log($"🏆 {currentPlayer?.tokenName} WINS THE GAME!");
    }



    public void showMap()
    {
        Debug.Log($"=== SHOWMAP: Creating {currentMap.allTiles.Count} tiles ===");
        Debug.Log($"tileToGenerate prefab: {(tileToGenerate != null ? tileToGenerate.name : "NULL")}");

        if (tileToGenerate == null)
        {
            Debug.LogError("tileToGenerate is NULL! Assign a prefab in Inspector!");
            return;
        }

        foreach (GameMapTile tileInMap in currentMap.allTiles)
        {
            Vector2 position = new Vector2(tileIncrementX * tileInMap.x, tileIncrementY * tileInMap.y);
            GameObject newTileObject = Instantiate(tileToGenerate, position, Quaternion.identity);
            newTileObject.name = $"Tile_{tileInMap.x}_{tileInMap.y}";

            SpriteRenderer sr = newTileObject.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = newTileObject.AddComponent<SpriteRenderer>();
                Debug.LogWarning($"Added SpriteRenderer to {newTileObject.name}");
            }

            if (sr.sprite == null)
            {
                sr.sprite = CreateDefaultSquareSprite();
            }

            Color originalColor;
            if (tileColor != null && tileInMap.tileType < tileColor.Length)
            {
                originalColor = tileColor[tileInMap.tileType];
            }
            else
            {
                originalColor = Color.gray;
            }
            sr.color = originalColor;
            sr.sortingOrder = 0;

            TileManager tileManager = newTileObject.GetComponent<TileManager>();
            if (tileManager == null)
            {
                tileManager = newTileObject.AddComponent<TileManager>();
            }

            tileManager.spriteRenderer = sr;
            allTiles.Add(tileManager);

            Color brighterColor = GetBrighterColor(originalColor);
            tileManager.highlightColor = brighterColor;

            tileInMap.SetTileType(tileInMap.tileType);

            tileManager.Initialize(tileInMap, originalColor);
            tileManager.OnTileClicked += OnTileClickedHandler;
        }

        Debug.Log($"Created {allTiles.Count} tiles successfully!");

        StartCoroutine(VerifyTilesVisible());
        StartCoroutine(SpawnUnitManagerDelayed());
    }

    private Sprite CreateDefaultSquareSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
    }

    IEnumerator VerifyTilesVisible()
    {
        yield return new WaitForSeconds(0.1f);

        TileManager[] tiles = FindObjectsByType<TileManager>(FindObjectsSortMode.None);
        Debug.Log($" Scene has {tiles.Length} TileManager objects");

        if (tiles.Length > 0)
        {
            TileManager firstTile = tiles[0];
            Debug.Log($"First tile position: {firstTile.transform.position}");
            Debug.Log($"First tile sprite: {(firstTile.spriteRenderer?.sprite != null ? firstTile.spriteRenderer.sprite.name : "NULL")}");
            Debug.Log($"First tile color: {firstTile.spriteRenderer?.color}");
            Debug.Log($"First tile active: {firstTile.gameObject.activeInHierarchy}");
        }

        if (Camera.main != null && tiles.Length > 0)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (TileManager tile in tiles)
            {
                Vector3 pos = tile.transform.position;
                minX = Mathf.Min(minX, pos.x);
                maxX = Mathf.Max(maxX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxY = Mathf.Max(maxY, pos.y);
            }

            float mapWidth = maxX - minX;
            float mapHeight = maxY - minY;

            Vector3 mapCenter = new Vector3(minX + mapWidth / 2f, minY + mapHeight / 2f, -10);
            Camera.main.transform.position = mapCenter;

            float requiredSizeByWidth = (mapWidth / 2f) / Camera.main.aspect + cameraPadding;
            float requiredSizeByHeight = (mapHeight / 2f) + cameraPadding;
            Camera.main.orthographicSize = Mathf.Max(requiredSizeByWidth, requiredSizeByHeight);
        }
    }

    IEnumerator SpawnUnitManagerDelayed()
    {
        yield return new WaitForSeconds(0.2f);

        UnitManager unitManager = FindFirstObjectByType<UnitManager>();
        if (unitManager == null)
        {
            GameObject unitManagerObj = new GameObject("UnitManager");
            unitManager = unitManagerObj.AddComponent<UnitManager>();
            Debug.Log("UnitManager created!");
        }
        else
        {
            unitManager.Reinitialize();
        }
    }

    private Color GetBrighterColor(Color color)
    {
        return new Color(
            Mathf.Min(color.r * highlightIntensity, 1f),
            Mathf.Min(color.g * highlightIntensity, 1f),
            Mathf.Min(color.b * highlightIntensity, 1f),
            color.a
        );
    }

    private void OnTileClickedHandler(GameMapTile clickedTile)
    {
        OnTileClickedEvent?.Invoke(clickedTile);

        switch (clickedTile.tileType)
        {
            case 0:
                Debug.Log($"<color=brown>BROWN tile at ({clickedTile.x}, {clickedTile.y})</color>");
                break;
            case 1:
                Debug.Log($"<color=silver>SILVER tile at ({clickedTile.x}, {clickedTile.y})</color>");
                break;
            case 2:
                Debug.Log($"<color=yellow>✨ GOLD tile at ({clickedTile.x}, {clickedTile.y}) ✨</color>");
                break;
        }
    }

    public void DamageTile(GameMapTile tile, int damage)
    {
        if (tile == null || tile.isDestroyed) return;

        tile.currentDefense -= damage;

        if (tile.currentDefense <= 0)
        {
            tile.currentDefense = 0;
            tile.isDestroyed = true;
            Debug.Log($"💥 TILE DESTROYED at ({tile.x}, {tile.y})");

            // CHECK WIN CONDITION
            CheckWinCondition(tile);
        }

        UpdateTileVisual(tile);
    }

    private void UpdateTileVisual(GameMapTile tile)
    {

        if (tile == winTile && winTileRevealed) return;

        TileManager[] tiles = FindObjectsByType<TileManager>(FindObjectsSortMode.None);
        foreach (TileManager tileManager in tiles)
        {
            if (tileManager.tileData == tile)
            {
                if (tile.isDestroyed)
                {
                    tileManager.SetTileColor(new Color(0.8f, 0.2f, 0.2f, 0.7f));
                    Collider2D col = tileManager.GetComponent<Collider2D>();
                    if (col != null) col.enabled = false;
                }
                else
                {
                    float healthPercent = (float)tile.currentDefense / tile.defense;
                    Color baseColor = tileColor[tile.tileType];
                    Color damagedColor = Color.Lerp(new Color(0.3f, 0.1f, 0.1f), baseColor, healthPercent);
                    tileManager.SetTileColor(damagedColor);
                }
                break;
            }
        }
    }

    public bool IsTileAvailable(int x, int y)
    {
        string key = $"{x}, {y}";
        if (currentMap.map.ContainsKey(key))
        {
            return !currentMap.map[key].isDestroyed;
        }
        return false;
    }

    public GameMapTile GetTileAt(int x, int y)
    {
        string key = $"{x}, {y}";
        if (currentMap.map.ContainsKey(key))
        {
            return currentMap.map[key];
        }
        return null;
    }

    public int GetTileDefense(int x, int y)
    {
        GameMapTile tile = GetTileAt(x, y);
        return tile != null ? tile.currentDefense : 0;
    }

    void CheckMouseInteractionComponents()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            Debug.LogError("❌ MISSING: EventSystem!");
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            if (mainCamera.GetComponent<Physics2DRaycaster>() == null)
            {
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            }
        }
    }

    void OnDestroy()
    {
        foreach (TileManager tile in allTiles)
        {
            if (tile != null)
                tile.OnTileClicked -= OnTileClickedHandler;
        }
        allTiles.Clear();
    }
}