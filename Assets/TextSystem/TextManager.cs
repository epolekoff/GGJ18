using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {

    public Canvas canvas;
    public Transform parent;
    public GameObject CenterBone;

    //How far away from the center should the textbox be?
    public float centerOffset = 40;

    //The amount we bump up the text scroll by default
    public float botOffset = 100;

    //This holds the value of the center of the canvas
    float center;

    //this list holds all the bubbles in the conversation
    List<TextBubble> playedBubbles;

	// Use this for initialization
	void Start () {

        if(CenterBone == null)
        {
            RectTransform objectRectTransform = canvas.GetComponent<RectTransform>();
            center = objectRectTransform.rect.width;
            center = center / 2;
        }
        else
        {
            center = CenterBone.transform.position.x;
        }
        
        playedBubbles = new List<TextBubble>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void SpawnBox(TextBubble tb, string message, bool isGreen)
    {
        TextBubble displayBubble = Instantiate(tb);
        displayBubble.transform.SetParent(parent ?? canvas.transform);
        Vector3 v3 = displayBubble.transform.position;
        Debug.Log(center);
        if(isGreen)
        {
            v3.x = center + centerOffset;
            v3.y += botOffset;
            if (tb.isLarge) v3.y += 50;
        } else
        {
            v3.x = center - centerOffset;
            v3.y += botOffset;
            if (tb.isLarge) v3.y += 10;
        }
        RectTransform rt = displayBubble.GetComponent<RectTransform>();
        ShiftTextBubbles(rt.rect.height);

        displayBubble.transform.position = v3;

        UnityEngine.UI.Text textObject = displayBubble.GetComponentInChildren<UnityEngine.UI.Text>();
        textObject.text = message;

        playedBubbles.Add(displayBubble);
    }

    void ShiftTextBubbles(float displacement)
    {
        foreach(TextBubble tb in playedBubbles)
        {
            Vector3 v3 = tb.transform.position;
            v3.y += displacement;   
            tb.transform.position = v3;
        }
    }

    //This is called from a !end command
    public void EndConversation()
    {
        //TODO: Should turn off the text message and go back to the contact list
    }
}
