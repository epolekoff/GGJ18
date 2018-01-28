using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {

    Canvas canvas;

    //How far away from the center should the textbox be?
    public float centerOffset = 40;

    //The amount we bump up the text scroll by default
    public float botOffset = 100;

    //This holds the value of the center of the canvas
    float center;


	// Use this for initialization
	void Start () {

        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            Debug.LogWarning(name + " cannot find a loaded canvas object");

        RectTransform objectRectTransform = canvas.GetComponent<RectTransform>();
        center = objectRectTransform.rect.width;
        center = center / 2;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void SpawnBox(TextBubble tb, string message, bool isGreen)
    {
        TextBubble displayBubble = Instantiate(tb);
        displayBubble.transform.SetParent(canvas.transform);
        Vector3 v3 = displayBubble.transform.position;
        Debug.Log(center);
        if(isGreen)
        {
            v3.x = center + centerOffset;
        } else
        {
            v3.x = center - centerOffset;
        }

        displayBubble.transform.position = v3;

        UnityEngine.UI.Text textObject = displayBubble.GetComponentInChildren<UnityEngine.UI.Text>();
        textObject.text = message;
    }
}
