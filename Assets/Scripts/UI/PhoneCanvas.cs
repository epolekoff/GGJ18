using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCanvas : MonoBehaviour {

    public GameObject HomeScreen;
    public GameObject ChatScreen;

    public Button[] MessageButtons;

    public GameObject GameButtonNotificationIcon;
    public GameObject ChatButtonNotificationIcon;

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
