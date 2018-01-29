using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageButton : MonoBehaviour {

    public Text NameText;
    public GameObject NewMessageIndicator;

    public bool Inactive { get; set; }

    public string ConversationFileName
    {
        get; set;
    }

    public bool HasBeenRead = false;

    public string GetPersonName()
    {
        return ConversationFileName.Substring(1, ConversationFileName.Length - 2);
    }

	public void SetData(string conversationFileName)
    {
        ConversationFileName = conversationFileName;
        NameText.text = GetPersonName();
        Inactive = false;
    }

    public void Read()
    {
        HasBeenRead = true;
        NewMessageIndicator.SetActive(false);
    }
}
