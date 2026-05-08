using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text dialogueText;
    public GameObject dialogueBox;

    [Header("Settings")]
    public float typingSpeed = 0.03f;
    public float messageDisplayTime = 3f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isTyping = false;

    void Start()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isTyping)
        {
            StartCoroutine(DisplayNextMessage());
        }
    }

    IEnumerator DisplayNextMessage()
    {
        isTyping = true;

        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            yield return StartCoroutine(TypeMessage(message));
            yield return new WaitForSeconds(messageDisplayTime);
        }

        isTyping = false;
    }

    IEnumerator TypeMessage(string message)
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";

            foreach (char letter in message.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

    public void ClearMessages()
    {
        messageQueue.Clear();
        if (dialogueText != null)
            dialogueText.text = "";
        isTyping = false;
    }
}