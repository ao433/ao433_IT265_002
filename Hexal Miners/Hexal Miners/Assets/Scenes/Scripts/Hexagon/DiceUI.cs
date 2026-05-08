using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    [Header("Dice References")]
    public Dice dice;
    public Button rollButton;
    public Text rollResultText;
    public Animator diceAnimator;

    [Header("UI Settings")]
    public bool autoHideResult = true;
    public float resultDisplayTime = 2f;

    void Start()
    {
        //button click event
        if (rollButton != null)
            rollButton.onClick.AddListener(OnRollButtonClicked);

  
        if (dice != null)
            dice.OnDiceRolled += OnDiceRolled;

        if (rollResultText != null && autoHideResult)
            rollResultText.gameObject.SetActive(false);
    }

    void OnRollButtonClicked()
    {
        if (dice != null && !dice.isRolling)
        {
        
            if (rollResultText != null)
                rollResultText.gameObject.SetActive(false);


            // Trigger dice animation if available
            if (diceAnimator != null)
                diceAnimator.SetTrigger("Roll");

          
            dice.RollDice();
        }
    }

    void OnDiceRolled(int rollValue)
    {
        // Display the result
        if (rollResultText != null)
        {
            rollResultText.text = $"Rolled: {rollValue}";
            rollResultText.gameObject.SetActive(true);

            if (autoHideResult)
                StartCoroutine(HideResultAfterDelay());
        }

        Debug.Log($"Dice roll result: {rollValue}");
    }

    IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayTime);
        if (rollResultText != null)
            rollResultText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {

        if (dice != null)
            dice.OnDiceRolled -= OnDiceRolled;

        if (rollButton != null)
            rollButton.onClick.RemoveListener(OnRollButtonClicked);
    }
}