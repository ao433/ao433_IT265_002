using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    [Header("Player Token Prefabs")]
    public GameObject bombPrefab;
    public GameObject pickaxePrefab;
    public GameObject lampPrefab;

    [Header("Spawn Positions")]
    public Vector2Int bombSpawnPos = new Vector2Int(0, 0);
    public Vector2Int pickaxeSpawnPos = new Vector2Int(2, 0);
    public Vector2Int lampSpawnPos = new Vector2Int(0, 2);

    [Header("Dice Reference")]
    public Dice dice;

    [Header("Card Deck Reference")]
    public CardDeckManager cardDeckManager;

    [Header("Game State")]
    public List<PlayerToken> allPlayers = new List<PlayerToken>();
    public int currentPlayerIndex = 0;
    public int currentRollValue = 0;

    public enum TurnStep { WaitingToRoll, WaitingToMove, WaitingToDraw }
    public TurnStep currentStep = TurnStep.WaitingToRoll;

    [Header("UI References")]
    public Text turnText;
    public Text instructionText;
    public DialogueManager dialogueManager;

    private GameMapManager mapManager;
    public List<Vector2Int> availableTiles = new List<Vector2Int>();

    private Dictionary<int, bool> playersSkippingTurn = new Dictionary<int, bool>();

    void Start()
    {
        mapManager = FindFirstObjectByType<GameMapManager>();

        if (cardDeckManager == null)
            cardDeckManager = FindFirstObjectByType<CardDeckManager>();

        if (dice != null)
            dice.OnDiceRolled += OnDiceRolled;

        if (mapManager != null)
            mapManager.OnTileClickedEvent += OnTileClicked;

        SpawnAllPlayers();
        StartTurn();
    }

    void SpawnAllPlayers()
    {
        if (bombPrefab != null)
            SpawnPlayer(bombPrefab, "Bomb", bombSpawnPos.x, bombSpawnPos.y, 1, new Color(0.8f, 0.2f, 0.2f));
        if (pickaxePrefab != null)
            SpawnPlayer(pickaxePrefab, "Pickaxe", pickaxeSpawnPos.x, pickaxeSpawnPos.y, 2, new Color(0.5f, 0.5f, 0.5f));
        if (lampPrefab != null)
            SpawnPlayer(lampPrefab, "Lamp", lampSpawnPos.x, lampSpawnPos.y, 3, new Color(1f, 0.9f, 0.2f));
    }

    void SpawnPlayer(GameObject prefab, string name, int startX, int startY, int playerNum, Color color)
    {
        Vector3 spawnPosition = GetTileWorldPosition(startX, startY);
        GameObject playerObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
        playerObj.name = $"{name}_Player{playerNum}";

        PlayerToken player = playerObj.GetComponent<PlayerToken>();
        if (player == null) player = playerObj.AddComponent<PlayerToken>();

        player.Initialize(name, startX, startY, playerNum, color);

        SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 100;

        allPlayers.Add(player);
    }

    Vector3 GetTileWorldPosition(int tileX, int tileY)
    {
        if (mapManager != null)
            return new Vector3(mapManager.tileIncrementX * tileX, mapManager.tileIncrementY * tileY, 0);
        return Vector3.zero;
    }

    void StartTurn()
    {
        PlayerToken currentPlayer = GetCurrentPlayer();
        if (currentPlayer == null) return;

        if (currentPlayer.isStunned)
        {
            if (dialogueManager != null)
                dialogueManager.AddMessage($"{currentPlayer.tokenName} is stunned and misses this turn!");

            currentPlayer.RecoverFromStun();
            currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
            StartTurn();
            return;
        }

        currentStep = TurnStep.WaitingToRoll;
        currentRollValue = 0;
        ClearAllHighlights();

        if (turnText != null)
            turnText.text = $"{currentPlayer.tokenName}'s Turn";

        if (instructionText != null)
            instructionText.text = "Click the dice to roll!";
    }

    void OnDiceRolled(int rollValue)
    {
        if (currentStep != TurnStep.WaitingToRoll) return;

        currentRollValue = rollValue;
        currentStep = TurnStep.WaitingToMove;

        PlayerToken currentPlayer = GetCurrentPlayer();

        if (instructionText != null)
            instructionText.text = $"You rolled {rollValue}! Click a green tile to move.";

        HighlightAvailableTiles(currentPlayer.currentX, currentPlayer.currentY, rollValue);
    }

    void HighlightAvailableTiles(int startX, int startY, int maxSteps)
    {
        ClearAllHighlights();
        availableTiles.Clear();

        for (int x = -maxSteps; x <= maxSteps; x++)
        {
            for (int y = -maxSteps; y <= maxSteps; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                if (distance <= maxSteps && distance > 0)
                {
                    int targetX = startX + x;
                    int targetY = startY + y;

                    if (IsValidTile(targetX, targetY))
                    {
                        availableTiles.Add(new Vector2Int(targetX, targetY));
                        HighlightTile(targetX, targetY, true);
                    }
                }
            }
        }
    }

    void OnTileClicked(GameMapTile clickedTile)
    {
        if (currentStep != TurnStep.WaitingToMove) return;

        if (clickedTile.isDestroyed)
        {
            if (dialogueManager != null)
                dialogueManager.AddMessage("That tile is destroyed! Choose another tile.");
            return;
        }

        int targetX = Mathf.RoundToInt(clickedTile.x);
        int targetY = Mathf.RoundToInt(clickedTile.y);
        Vector2Int target = new Vector2Int(targetX, targetY);

        if (availableTiles.Contains(target))
        {
            PlayerToken currentPlayer = GetCurrentPlayer();
            PlayerToken existingPlayer = GetPlayerOnTile(targetX, targetY);

            currentPlayer.MoveTo(targetX, targetY);
            ClearAllHighlights();
            currentStep = TurnStep.WaitingToDraw;

            if (existingPlayer != null && existingPlayer != currentPlayer)
            {
                if (dialogueManager != null)
                    dialogueManager.AddMessage($"You're now on the same tile as {existingPlayer.tokenName}!");
            }

            if (instructionText != null)
                instructionText.text = "Click DRAW to get a card!";
        }
    }

    public PlayerToken GetPlayerOnTile(int x, int y)
    {
        foreach (PlayerToken player in allPlayers)
        {
            if (player.currentX == x && player.currentY == y)
            {
                return player;
            }
        }
        return null;
    }

    void HighlightTile(int x, int y, bool highlight)
    {
        if (mapManager != null && mapManager.currentMap != null)
        {
            string key = $"{x}, {y}";
            if (mapManager.currentMap.map.ContainsKey(key))
            {
                GameMapTile tile = mapManager.currentMap.map[key];
                if (tile.isDestroyed) return;

                TileManager[] tiles = FindObjectsByType<TileManager>(FindObjectsSortMode.None);
                foreach (TileManager tileManager in tiles)
                {
                    if (tileManager.tileData == tile)
                    {
                        if (highlight)
                            tileManager.SetHighlightColor(new Color(0, 1, 0, 0.5f));
                        else
                            tileManager.ResetToOriginalColor();
                        break;
                    }
                }
            }
        }
    }

    void ClearAllHighlights()
    {
        foreach (Vector2Int tile in availableTiles)
            HighlightTile(tile.x, tile.y, false);
        availableTiles.Clear();
    }

    bool IsValidTile(int x, int y)
    {
        if (mapManager != null && mapManager.currentMap != null)
            return mapManager.IsTileAvailable(x, y);
        return false;
    }

    public void OnCardDrawn()
    {
        if (currentStep != TurnStep.WaitingToDraw) return;

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;

        if (cardDeckManager != null)
            cardDeckManager.ResetDrawForPlayer(GetCurrentPlayer().playerNumber);

        StartTurn();
    }

    public PlayerToken GetCurrentPlayer()
    {
        if (currentPlayerIndex < allPlayers.Count)
            return allPlayers[currentPlayerIndex];
        return null;
    }

    public bool CanDrawCard(int playerNumber)
    {
        PlayerToken currentPlayer = GetCurrentPlayer();
        return currentPlayer != null &&
               currentPlayer.playerNumber == playerNumber &&
               currentStep == TurnStep.WaitingToDraw;
    }

    public void EndTurnNow()
    {
        PlayerToken currentPlayer = GetCurrentPlayer();

        if (cardDeckManager != null)
            cardDeckManager.ForceCleanup();

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        currentStep = TurnStep.WaitingToRoll;
        currentRollValue = 0;
        ClearAllHighlights();

        if (cardDeckManager != null)
            cardDeckManager.ResetDrawForPlayer(GetCurrentPlayer().playerNumber);

        StartTurn();
    }

    public void HandleCardUse(CardDeckManager.WeightedCard card)
    {
        PlayerToken currentPlayer = GetCurrentPlayer();
        if (currentPlayer == null || card == null) return;

        int damage = card.hitBonus;

        if (damage > 0 && mapManager != null)
        {
            GameMapTile currentTile = mapManager.GetTileAt(currentPlayer.currentX, currentPlayer.currentY);
            if (currentTile != null && !currentTile.isDestroyed)
            {
                mapManager.DamageTile(currentTile, damage);
                if (dialogueManager != null)
                {
                    if (currentTile.isDestroyed)
                        dialogueManager.AddMessage("Tile destroyed!");
                    else
                        dialogueManager.AddMessage($"Tile damaged! Defense: {currentTile.currentDefense}/{currentTile.defense}");
                }
            }
        }

        if (card.defenseBonus > 0)
            currentPlayer.AddDefense(card.defenseBonus);

        if (card.cardType == CardDeckManager.CardType.Lamp)
        {
            if (dialogueManager != null)
                dialogueManager.AddMessage("Area illuminated!");
        }

        EndTurnAfterCardAction();
    }

    public void HandleCardStore(CardDeckManager.WeightedCard card)
    {
        PlayerToken currentPlayer = GetCurrentPlayer();
        if (currentPlayer == null || card == null) return;

        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory != null)
            inventory.AddCard(currentPlayer.playerNumber, card);

        EndTurnAfterCardAction();
    }

    public void HandleDoNothing()
    {
        EndTurnAfterCardAction();
    }

    private void AttackTilePlayerIsOn(PlayerToken player)
    {
        if (player == null || mapManager == null) return;

        GameMapTile currentTile = mapManager.GetTileAt(player.currentX, player.currentY);
        if (currentTile != null && !currentTile.isDestroyed)
        {
            int damage = player.GetHitPower();
            mapManager.DamageTile(currentTile, damage);

            if (currentTile.isDestroyed && dialogueManager != null)
                dialogueManager.AddMessage("The tile was destroyed!");
        }
    }

    private void EndTurnAfterCardAction()
    {
        currentStep = TurnStep.WaitingToRoll;
        currentRollValue = 0;
        ClearAllHighlights();

        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;

        if (cardDeckManager != null)
            cardDeckManager.ResetDrawForPlayer(GetCurrentPlayer().playerNumber);

        StartTurn();
    }

    public void StoreCardEndTurn()
    {
        PlayerToken currentPlayer = GetCurrentPlayer();
        if (currentPlayer == null) return;

        if (cardDeckManager != null)
        {
            var card = cardDeckManager.GetCurrentDrawnCard();
            if (card != null)
            {
                PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.AddCard(currentPlayer.playerNumber, card);
                }
            }
            cardDeckManager.ForceCleanup();
        }

        EndTurnNow();
    }

    public void Reinitialize()
    {
        foreach (PlayerToken player in allPlayers)
        {
            if (player != null)
                Destroy(player.gameObject);
        }
        allPlayers.Clear();

        currentPlayerIndex = 0;
        currentRollValue = 0;
        currentStep = TurnStep.WaitingToRoll;

        playersSkippingTurn.Clear();
        ClearAllHighlights();
        SpawnAllPlayers();
        StartTurn();
    }

    void OnDestroy()
    {
        if (dice != null)
            dice.OnDiceRolled -= OnDiceRolled;
        if (mapManager != null)
            mapManager.OnTileClickedEvent -= OnTileClicked;
    }
}