using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CardDeckManager : MonoBehaviour
{
    [Header("UI References")]
    public Image cardImage;
    public Button drawButton;

    // Action Buttons
    public GameObject actionPanel;
    public Button useButton;
    public Button storeButton;
    public Button doNothingButton;

    [Header("Card Sprites")]
    public Sprite woodenPickaxeSprite;
    public Sprite ironPickaxeSprite;
    public Sprite goldenPickaxeSprite;
    public Sprite regularTNTSprite;
    public Sprite superbombTNTSprite;
    public Sprite purpleTNTSprite;
    public Sprite colorfulTNTSprite;
    public Sprite woodenHelmetSprite;
    public Sprite ironHelmetSprite;
    public Sprite lampSprite;

    [Header("Card Settings")]
    public List<WeightedCard> weightedCards = new List<WeightedCard>();

    private HashSet<int> playersWhoDrewThisTurn = new HashSet<int>();
    private Sprite defaultCardBack;
    private WeightedCard currentDrawnCard;
    private bool waitingForAction = false;

    [System.Serializable]
    public class WeightedCard
    {
        public string cardName;
        public string description;
        public Sprite cardSprite;
        public CardType cardType;
        [Range(0f, 100f)]
        public float drawChance = 10f;
        public int hitBonus = 0;
        public int defenseBonus = 0;
    }

    public enum CardType
    {
        Pickaxe,
        TNT,
        Helmet,
        Lamp
    }

    private UnitManager unitManager;
    private PlayerInventory playerInventory;

    void Start()
    {
        unitManager = FindFirstObjectByType<UnitManager>();
        playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (cardImage != null)
            defaultCardBack = cardImage.sprite;

        if (weightedCards.Count == 0)
            InitializeDefaultCards();

        if (drawButton != null)
        {
            drawButton.onClick.RemoveAllListeners();
            drawButton.onClick.AddListener(TryDrawCard);
            drawButton.interactable = false;
        }

        if (actionPanel != null)
            actionPanel.SetActive(false);
        
        if (weightedCards.Count == 0)
            InitializeDefaultCards();
        else
            Debug.Log($"🃏 Using {weightedCards.Count} cards from Inspector (NOT overriding)");

    }

    public void OnUseClicked()
    {
        if (currentDrawnCard != null && unitManager != null)
        {
            unitManager.HandleCardUse(currentDrawnCard);
        }
        ForceCleanup();
    }

    public void OnStoreClicked()
    {
        if (currentDrawnCard != null && unitManager != null)
        {
            PlayerToken currentPlayer = unitManager.GetCurrentPlayer();
            if (playerInventory != null)
            {
                playerInventory.AddCard(currentPlayer.playerNumber, currentDrawnCard);
            }
            unitManager.EndTurnNow();
        }
        ForceCleanup();
    }

    public void OnDoNothingClicked()
    {
        if (unitManager != null)
        {
            unitManager.HandleDoNothing();
        }
        ForceCleanup();
    }

    public void ForceCleanup()
    {
        waitingForAction = false;
        currentDrawnCard = null;

        if (actionPanel != null)
            actionPanel.SetActive(false);

        if (cardImage != null && defaultCardBack != null)
            cardImage.sprite = defaultCardBack;
    }

    void InitializeDefaultCards()
    {
       
        if (weightedCards.Count > 0)
        {
            Debug.Log($"🃏 Using {weightedCards.Count} cards from Inspector settings");
            return;
        }

    
        weightedCards.Add(new WeightedCard { cardName = "Wooden Pickaxe", description = "+1 Hit", cardSprite = woodenPickaxeSprite, cardType = CardType.Pickaxe, hitBonus = 1, drawChance = 33f });
        weightedCards.Add(new WeightedCard { cardName = "Iron Pickaxe", description = "+2 Hit", cardSprite = ironPickaxeSprite, cardType = CardType.Pickaxe, hitBonus = 2, drawChance = 34f });
        weightedCards.Add(new WeightedCard { cardName = "Golden Pickaxe", description = "+3 Hit", cardSprite = goldenPickaxeSprite, cardType = CardType.Pickaxe, hitBonus = 3, drawChance = 33f });
        weightedCards.Add(new WeightedCard { cardName = "Regular TNT", description = "+2 Hit", cardSprite = regularTNTSprite, cardType = CardType.TNT, hitBonus = 2, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Superbomb TNT", description = "+4 Hit", cardSprite = superbombTNTSprite, cardType = CardType.TNT, hitBonus = 4, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Purple TNT", description = "+3 Hit", cardSprite = purpleTNTSprite, cardType = CardType.TNT, hitBonus = 3, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Colorful TNT", description = "+3 Hit", cardSprite = colorfulTNTSprite, cardType = CardType.TNT, hitBonus = 3, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Wooden Helmet", description = "+1 Def", cardSprite = woodenHelmetSprite, cardType = CardType.Helmet, defenseBonus = 1, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Iron Helmet", description = "+2 Def", cardSprite = ironHelmetSprite, cardType = CardType.Helmet, defenseBonus = 2, drawChance = 0f });
        weightedCards.Add(new WeightedCard { cardName = "Lamp", description = "Illuminate", cardSprite = lampSprite, cardType = CardType.Lamp, drawChance = 0f });
    }

    public WeightedCard GetCurrentDrawnCard()
    {
        return currentDrawnCard;
    }

    void Update()
    {
        // Press M to end turn (Do Nothing)
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            OnDoNothingClicked();
        }

        // Press N to store card
        if (Keyboard.current != null && Keyboard.current.nKey.wasPressedThisFrame)
        {
            OnStoreClicked();
        }

        // Press B to USE card (mine the tile)
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            Debug.Log("⚔️ B KEY - Using card to attack tile!");
            OnUseClicked();
        }

        // Press K to toggle inventory
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
            if (inventory != null)
            {
                PlayerToken player = unitManager?.GetCurrentPlayer();
                if (player != null)
                {
                    inventory.ToggleInventory(player.playerNumber);
                }
            }
        }

        // Manage draw button
        if (unitManager != null && drawButton != null)
        {
            PlayerToken currentPlayer = unitManager.GetCurrentPlayer();
            bool shouldEnable = currentPlayer != null &&
                               unitManager.CanDrawCard(currentPlayer.playerNumber) &&
                               !waitingForAction &&
                               !playersWhoDrewThisTurn.Contains(currentPlayer.playerNumber);

            drawButton.interactable = shouldEnable;
        }
    }
    public void TryDrawCard()
    {
        PlayerToken currentPlayer = unitManager?.GetCurrentPlayer();
        if (currentPlayer == null) return;

        currentDrawnCard = GetRandomCard();
        if (currentDrawnCard == null) return;

        playersWhoDrewThisTurn.Add(currentPlayer.playerNumber);

        if (cardImage != null && currentDrawnCard.cardSprite != null)
            cardImage.sprite = currentDrawnCard.cardSprite;

        waitingForAction = true;
        if (actionPanel != null)
            actionPanel.SetActive(true);

        drawButton.interactable = false;
    }

    WeightedCard GetRandomCard()
    {
        if (weightedCards.Count == 0) return null;

        // Print all chances for debugging
        float totalWeight = 0f;
        foreach (var card in weightedCards)
        {
            totalWeight += card.drawChance;
            Debug.Log($"📊 {card.cardName}: chance={card.drawChance}%");
        }
        Debug.Log($"📊 TOTAL: {totalWeight}%");

        float random = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var card in weightedCards)
        {
            cumulative += card.drawChance;
            if (random <= cumulative)
            {
                Debug.Log($"🎯 DREW: {card.cardName} (random={random}, cumulative={cumulative})");
                return card;
            }
        }

        return weightedCards[0];
    }

    public void ResetDrawForPlayer(int playerNumber)
    {
        playersWhoDrewThisTurn.Remove(playerNumber);
        waitingForAction = false;
        currentDrawnCard = null;
        if (actionPanel != null) actionPanel.SetActive(false);
    }

    public void ResetAllDrawStatus()
    {
        playersWhoDrewThisTurn.Clear();
        waitingForAction = false;
        currentDrawnCard = null;
        if (actionPanel != null) actionPanel.SetActive(false);
    }
}