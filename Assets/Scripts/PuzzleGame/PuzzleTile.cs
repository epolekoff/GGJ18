using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PuzzleTileType
{
    None,
    Red,
    Blue,
    Yellow,
    Green,
    Purple,
    Alarm,
    Junk
}

public class PuzzleTile : MonoBehaviour {

    public int X;
    public int Y;

    public PuzzleTileType PuzzleTileType = PuzzleTileType.None;

    public bool CanBeMatched = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
