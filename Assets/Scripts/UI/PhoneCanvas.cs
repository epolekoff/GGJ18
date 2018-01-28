using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PhoneCanvas : MonoBehaviour {

    public GameObject HomeScreen;
    public GameObject ChatScreen;

    public MessageButton[] MessageButtons;
    public TextConversation TextConversationPrefab;

    public GameObject GameButtonNotificationIcon;
    public GameObject ChatButtonNotificationIcon;

    public GameObject GameplayBackground;
    public TerminalWindowText TerminalWindowText;

    public bool HasUnreadMessages = true;

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
        UpdateChatButtons();
        HasUnreadMessages = false;
    }

    /// <summary>
    /// Show the chat buttons we need.
    /// </summary>
    public void UpdateChatButtons()
    {
        // Show the buttons we want.
        var conversations = ProgressManager.Instance.GetCurrentTextConversationsForLevel(GameManager.Instance.CurrentLevel);
        int numButtons = conversations.Count;

        for (int i = 0; i < MessageButtons.Length; i++)
        {
            bool active = i < numButtons;
            MessageButtons[i].Inactive = !active;
            MessageButtons[i].gameObject.SetActive(active);
            if(i < numButtons)
            {
                MessageButtons[i].SetData(conversations[i]);
                MessageButtons[i].NewMessageIndicator.SetActive(!MessageButtons[i].HasBeenRead);
            }
        }
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

        // Get the list of conversations so we know what to display
        var conversations = ProgressManager.Instance.GetCurrentTextConversationsForLevel(GameManager.Instance.CurrentLevel);

        // Set data on the chat canvas.
        GameManager.Instance.ChatCanvas.NameText.text = MessageButtons[index].GetPersonName();

        // Create the conversation
        TextManager.Instance.ClearConversation();
        var conversationObj = GameObject.Instantiate(TextConversationPrefab, GameManager.Instance.ChatCanvas.transform);
        TextConversation conversation = conversationObj.GetComponent<TextConversation>();

        conversation.Initialize(GameManager.Instance.ChatCanvas.GetComponent<Canvas>(), MessageButtons[index].ConversationFileName);

        MessageButtons[index].Read();

        // If all of the chats at this level have been read, advance the dialog.
        if(MessageButtons.ToList().All(b => b.HasBeenRead || b.Inactive))
        {
            ProgressManager.Instance.AdvanceDialogProgress();
            foreach(var button in MessageButtons)
            {
                button.HasBeenRead = false;
            }
            
        }
        
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
