using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Dice : MonoBehaviour
{
    [Header("Dice Settings")]
    public int currentRollValue = 1;
    public bool isRolling = false;

    [Header("Dice Visuals")]
    public Image diceImage; // UI Image for the dice
    public Sprite[] diceFaces; // Array of 6 sprites (faces 1-6)
    public float rollDuration = 0.5f;
    public float rollSpeed = 0.05f;

    [Header("Roll Animation")]
    public AnimationCurve rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI References")]
    public Button rollButton; 
    public TMP_Text resultText; 

    [Header("Events")]
    public System.Action<int> OnDiceRolled;

    private Coroutine currentRollCoroutine;
    private Coroutine currentButtonResetCoroutine;
    private Coroutine currentResultHideCoroutine;
    private string originalButtonText = "ROLL";

    void Start()
    {
   
        UpdateDiceFace(currentRollValue);

        // Store original button text
        if (rollButton != null)
        {
            TMP_Text buttonText = rollButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                originalButtonText = buttonText.text;
            }

            
            rollButton.onClick.AddListener(() => {
                RollDice();
            });
        }

       
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }
    }

  
    public void RollDice()
    {
        if (isRolling) return;

        if (currentRollCoroutine != null)
            StopCoroutine(currentRollCoroutine);

        currentRollCoroutine = StartCoroutine(RollDiceAnimation());
    }

   
    public void RollDice(int minValue, int maxValue)
    {
        if (isRolling) return;

        StartCoroutine(RollDiceAnimation(minValue, maxValue));
    }

    // Main roll animation coroutine (standard 1-6)
    IEnumerator RollDiceAnimation()
    {
        yield return RollDiceAnimation(1, 7); // 1 to 6 (max exclusive)
    }

    // Main roll animation coroutine with custom range
    IEnumerator RollDiceAnimation(int minValue, int maxValue)
    {
        isRolling = true;
        float elapsedTime = 0f;
        int finalValue = 0;

        // Hide result text during roll and cancel any pending coroutines
        if (resultText != null)
        {
            if (currentResultHideCoroutine != null)
                StopCoroutine(currentResultHideCoroutine);
            resultText.gameObject.SetActive(false);
        }

        if (currentButtonResetCoroutine != null)
            StopCoroutine(currentButtonResetCoroutine);

     
        while (elapsedTime < rollDuration)
        {
            // Random value for animation
            int randomValue = Random.Range(minValue, maxValue);
            UpdateDiceFace(randomValue);

            // Wait for next frame (faster animation)
            yield return new WaitForSeconds(rollSpeed);
            elapsedTime += rollSpeed;
        }

      
        finalValue = Random.Range(minValue, maxValue);
        currentRollValue = finalValue;
        UpdateDiceFace(finalValue);

        isRolling = false;


        OnDiceRolled?.Invoke(finalValue);

     
        ShowRollResult(finalValue);

        Debug.Log($"Dice rolled: {finalValue}");
    }


    private void ShowRollResult(int finalValue)
    {
 
        if (rollButton != null)
        {
            TMP_Text buttonText = rollButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = $"Rolled: {finalValue}";
                currentButtonResetCoroutine = StartCoroutine(ResetButtonText(buttonText));
            }
        }

        if (resultText != null)
        {
            resultText.text = $"Rolled: {finalValue}";
            resultText.gameObject.SetActive(true);
            currentResultHideCoroutine = StartCoroutine(HideResultTextAfterDelay());
        }
    }

    IEnumerator ResetButtonText(TMP_Text buttonText)
    {
        yield return new WaitForSeconds(2f);
        if (buttonText != null && !isRolling) // Only reset if not currently rolling
        {
            buttonText.text = originalButtonText;
        }
        currentButtonResetCoroutine = null;
    }

    IEnumerator HideResultTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        if (resultText != null && !isRolling) // Only hide if not currently rolling
        {
            resultText.gameObject.SetActive(false);
        }
        currentResultHideCoroutine = null;
    }


    void UpdateDiceFace(int value)
    {
        currentRollValue = value;

        if (diceImage != null && diceFaces != null && value >= 1 && value <= diceFaces.Length)
        {
            diceImage.sprite = diceFaces[value - 1];
        }
    }


    public void SetDiceValue(int value)
    {
        if (value >= 1 && value <= 6)
        {
            currentRollValue = value;
            UpdateDiceFace(value);
        }
    }

    void OnDestroy()
    {

        if (rollButton != null)
        {
            rollButton.onClick.RemoveAllListeners();
        }

        // Stop all coroutines
        if (currentButtonResetCoroutine != null)
            StopCoroutine(currentButtonResetCoroutine);
        if (currentResultHideCoroutine != null)
            StopCoroutine(currentResultHideCoroutine);
        if (currentRollCoroutine != null)
            StopCoroutine(currentRollCoroutine);
    }
}