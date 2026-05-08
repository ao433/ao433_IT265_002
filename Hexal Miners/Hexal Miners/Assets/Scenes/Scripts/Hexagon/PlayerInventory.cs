using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class StoredCard
    {
        public string cardName;
        public string description;
        public Sprite cardSprite;
        public CardDeckManager.CardType cardType;
        public int hitBonus;
        public int defenseBonus;
    }

    [System.Serializable]
    public class PlayerInventoryData
    {
        public int playerNumber;
        public List<StoredCard> storedCards = new List<StoredCard>();
        public int selectedCardIndex = -1;
    }

    [Header("Inventory UI")]
    public GameObject inventoryPanel;
    public Transform cardContainer;
    public GameObject cardButtonPrefab;
    public Button closeButton;
    public TMP_Text playerNameText;
    public TMP_Text cardCountText;
    public Button useSelectedButton;

    [Header("Settings")]
    public KeyCode inventoryToggleKey = KeyCode.I;

    private List<PlayerInventoryData> allInventories = new List<PlayerInventoryData>();
    private UnitManager unitManager;
    private CardDeckManager cardDeckManager;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        unitManager = FindFirstObjectByType<UnitManager>();
        cardDeckManager = FindFirstObjectByType<CardDeckManager>();

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideInventory);

        if (useSelectedButton != null)
            useSelectedButton.onClick.AddListener(UseSelectedCard);
    }

    // Replace the Update() method:
    void Update()
    {
        // Toggle inventory with key (New Input System)
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            PlayerToken currentPlayer = unitManager?.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                ToggleInventory(currentPlayer.playerNumber);
            }
        }
    }

    public void AddCard(int playerNumber, CardDeckManager.WeightedCard card)
    {
        PlayerInventoryData data = GetInventoryForPlayer(playerNumber);
        if (data == null)
        {
            data = new PlayerInventoryData { playerNumber = playerNumber };
            allInventories.Add(data);
        }

        StoredCard storedCard = new StoredCard
        {
            cardName = card.cardName,
            description = card.description,
            cardSprite = card.cardSprite,
            cardType = card.cardType,
            hitBonus = card.hitBonus,
            defenseBonus = card.defenseBonus
        };

        data.storedCards.Add(storedCard);
        Debug.Log($"📦 Player {playerNumber} stored {card.cardName}. Total cards: {data.storedCards.Count}");
    }

    public void ShowInventory(int playerNumber)
    {
        if (inventoryPanel == null) return;

        PlayerInventoryData data = GetInventoryForPlayer(playerNumber);
        if (data == null) return;

        // Clear old images
        foreach (GameObject obj in spawnedButtons)
            Destroy(obj);
        spawnedButtons.Clear();

        // Update UI text
        if (playerNameText != null)
        {
            PlayerToken player = unitManager?.GetCurrentPlayer();
            playerNameText.text = $"{player?.tokenName}'s Inventory";
        }

        if (cardCountText != null)
            cardCountText.text = $"{data.storedCards.Count} Cards";

        // Create card images
        for (int i = 0; i < data.storedCards.Count; i++)
        {
            StoredCard card = data.storedCards[i];
            int index = i;

            // Create a simple Image instead of a Button
            GameObject imageObj = new GameObject($"Card_{i}");
            imageObj.transform.SetParent(cardContainer, false);

            Image img = imageObj.AddComponent<Image>();
            if (card.cardSprite != null)
            {
                img.sprite = card.cardSprite;
            }

            // Add Button component for clicking
            Button btn = imageObj.AddComponent<Button>();
            btn.onClick.AddListener(() => SelectCard(playerNumber, index));

            // Set size
            RectTransform rect = imageObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 100);

            // Highlight if selected
            if (index == data.selectedCardIndex)
            {
                img.color = Color.yellow;
            }

            spawnedButtons.Add(imageObj);
        }

        inventoryPanel.SetActive(true);
    }

    public void HideInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    public void ToggleInventory(int playerNumber)
    {
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            HideInventory();
        }
        else
        {
            ShowInventory(playerNumber);
        }
    }

    void SelectCard(int playerNumber, int cardIndex)
    {
        PlayerInventoryData data = GetInventoryForPlayer(playerNumber);
        if (data == null) return;

        data.selectedCardIndex = cardIndex;
        Debug.Log($"📦 Player {playerNumber} selected card at index {cardIndex}: {data.storedCards[cardIndex].cardName}");

        RefreshInventoryDisplay(playerNumber);
    }

    void RefreshInventoryDisplay(int playerNumber)
    {
        PlayerInventoryData data = GetInventoryForPlayer(playerNumber);
        if (data == null) return;

        foreach (GameObject obj in spawnedButtons)
            Destroy(obj);
        spawnedButtons.Clear();

        for (int i = 0; i < data.storedCards.Count; i++)
        {
            StoredCard card = data.storedCards[i];
            int index = i;

            GameObject imageObj = new GameObject($"Card_{i}");
            imageObj.transform.SetParent(cardContainer, false);

            Image img = imageObj.AddComponent<Image>();
            if (card.cardSprite != null)
            {
                img.sprite = card.cardSprite;
            }

            Button btn = imageObj.AddComponent<Button>();
            btn.onClick.AddListener(() => SelectCard(playerNumber, index));

            RectTransform rect = imageObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 100);

            if (index == data.selectedCardIndex)
            {
                img.color = Color.yellow;
            }

            spawnedButtons.Add(imageObj);
        }
    }

    public void UseSelectedCard()
    {
        PlayerToken currentPlayer = unitManager?.GetCurrentPlayer();
        if (currentPlayer == null) return;

        PlayerInventoryData data = GetInventoryForPlayer(currentPlayer.playerNumber);
        if (data == null || data.selectedCardIndex < 0 || data.selectedCardIndex >= data.storedCards.Count)
        {
            Debug.LogWarning("No card selected!");
            return;
        }

        StoredCard selectedCard = data.storedCards[data.selectedCardIndex];

        // Apply the card effect AND attack the tile
        if (unitManager != null)
        {
            CardDeckManager.WeightedCard card = new CardDeckManager.WeightedCard
            {
                cardName = selectedCard.cardName,
                description = selectedCard.description,
                cardSprite = selectedCard.cardSprite,
                cardType = selectedCard.cardType,
                hitBonus = selectedCard.hitBonus,
                defenseBonus = selectedCard.defenseBonus
            };

            unitManager.HandleCardUse(card);
        }

        // Remove the card from inventory
        data.storedCards.RemoveAt(data.selectedCardIndex);
        data.selectedCardIndex = -1;

        // Hide inventory
        HideInventory();

        Debug.Log($"✅ Player {currentPlayer.playerNumber} used stored {selectedCard.cardName}");
    }

    private PlayerInventoryData GetInventoryForPlayer(int playerNumber)
    {
        return allInventories.Find(i => i.playerNumber == playerNumber);
    }

    public int GetCardCount(int playerNumber)
    {
        PlayerInventoryData data = GetInventoryForPlayer(playerNumber);
        return data != null ? data.storedCards.Count : 0;
    }
}