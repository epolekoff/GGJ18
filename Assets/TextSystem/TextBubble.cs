using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBubble : MonoBehaviour {

    //public string bubbleName;

    
    public string displayText;

    public SpriteRenderer bubbleSprite;

    private TextMesh textMesh;

	// Use this for initialization
	void Start () {
        textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh == null)
            Debug.LogWarning(gameObject.name + " is missing it's TextMesh component.");


        SpriteRenderer bubble = Instantiate(bubbleSprite);
        bubble.gameObject.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
