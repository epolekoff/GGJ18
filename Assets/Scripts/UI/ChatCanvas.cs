using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatCanvas : MonoBehaviour {

    public Text NameText;
    public GameObject ChatBubbleParent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReturnToChatMenu()
    {
        // Delete all of the chat bubbles
        foreach(Transform child in ChatBubbleParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Hide this canvas
        gameObject.SetActive(false);

        // Show the phone screen again.
        GameManager.Instance.PhoneCanvas.ChatScreen.SetActive(true);
    }
}
