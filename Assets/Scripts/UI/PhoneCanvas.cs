using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCanvas : MonoBehaviour {

    public GameObject HomeScreen;
    public GameObject ChatScreen;

    public Button[] MessageButtons;
    public TextConversation TextConversationPrefab;

    public GameObject GameButtonNotificationIcon;
    public GameObject ChatButtonNotificationIcon;

    public GameObject GameplayBackground;
    public TerminalWindowText TerminalWindowText;

    /// <summary>
    /// Open the game.
    /// </summary>
    public void OnGameButtonPressed()
    {
        GameManager.Instance.OnGameButtonPressed();
    }

    /// <summary>
    /// Open the chat window.
    /// </summary>
    public void OnChatButtonPressed()
    {
        GameManager.Instance.OnChatButtonPressed();
    }

    /// <summary>
    /// Return the Home Screen
    /// </summary>
    public void ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }

    /// <summary>
    /// When a chat message is pressed, show the chat.
    /// </summary>
    /// <param name="index"></param>
    public void OnMessageButtonPressed(int index)
    {
        // Enable the chat canvas.
        GameManager.Instance.ChatCanvas.gameObject.SetActive(true);
        ChatScreen.SetActive(false);

        // Set data on the chat canvas.
        GameManager.Instance.ChatCanvas.NameText.text = "Dynamic Name";

        // Create the conversation
        var conversationObj = GameObject.Instantiate(TextConversationPrefab, GameManager.Instance.ChatCanvas.transform);
        TextConversation conversation = conversationObj.GetComponent<TextConversation>();
        conversation.Initialize(GameManager.Instance.ChatCanvas.GetComponent<Canvas>(), ProgressManager.Instance.GetCurrentTextConversationFile(index));
    }

    /// <summary>
    /// Show notifications
    /// </summary>
    /// <param name="show"></param>
    /// <param name="number"></param>
    public void ShowGameButtonNotification(bool show, int number = 0)
    {
        GameButtonNotificationIcon.SetActive(show);
        if(number > 0)
        {
            GameButtonNotificationIcon.GetComponentInChildren<Text>().text = "" + number;
        }
    }

    /// <summary>
    /// Show notifications
    /// </summary>
    /// <param name="show"></param>
    /// <param name="number"></param>
    public void ShowChatButtonNotification(bool show, int number = 0)
    {
        ChatButtonNotificationIcon.SetActive(show);
        if (number > 0)
        {
            ChatButtonNotificationIcon.GetComponentInChildren<Text>().text = "" + number;
        }
    }
}
