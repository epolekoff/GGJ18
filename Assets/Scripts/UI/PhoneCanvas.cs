using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGameButtonPressed()
    {
        GameManager.Instance.OnGameButtonPressed();
    }

    public void OnChatButtonPressed()
    {
        GameManager.Instance.OnChatButtonPressed();
    }
}
