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

    public int X { get; set; }
    public int Y { get; set; }

    public GameObject Visual;

    public PuzzleTileType PuzzleTileType = PuzzleTileType.None;

    public bool CanBeMatched = true;

    private const float SelectedResizeSpeed = 0.2f;
    private const float SelectedPositionZ = 10f;
    private const float SelectedSizeModifier = 0.2f;

    private Vector3 m_initialScale;
    private float m_initialPositionZ;
    private Coroutine m_growCoroutine;
    private Coroutine m_shrinkCoroutine;

    // Use this for initialization
    void Start () {
        m_initialPositionZ = transform.localPosition.z;
        m_initialScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseDrag()
    {
        
    }

    void OnMouseUp()
    {
        m_shrinkCoroutine = StartCoroutine(ShrinkVisual());
        PuzzleManager.Instance.EnableDragCursor(false);
        PuzzleManager.Instance.SwapWithCurrentHoveredTile(this);
    }

    void OnMouseDown()
    {
        Debug.Log("X: " + X + ", Y: " + Y);
        m_growCoroutine = StartCoroutine(GrowVisual());
        PuzzleManager.Instance.EnableDragCursor(true);
    }

    void OnMouseEnter()
    {
        PuzzleManager.Instance.SetCurrentHoveredTile(this);
    }

    private IEnumerator GrowVisual()
    {
        float timer = SelectedResizeSpeed;
        while(timer > 0)
        {
            timer -= Time.deltaTime;

            float ratio = 1-(timer / SelectedResizeSpeed);

            Visual.transform.localPosition = new Vector3(
                Visual.transform.localPosition.x,
                Visual.transform.localPosition.y,
                m_initialPositionZ - (SelectedPositionZ * ratio));
            Visual.transform.localScale = m_initialScale + (Vector3.one * SelectedSizeModifier * ratio);

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ShrinkVisual()
    {
        float timer = SelectedResizeSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            float ratio = timer / SelectedResizeSpeed;

            Visual.transform.localPosition = new Vector3(
                Visual.transform.localPosition.x,
                Visual.transform.localPosition.y,
                m_initialPositionZ - (SelectedPositionZ * ratio));
            Visual.transform.localScale = m_initialScale + (Vector3.one * SelectedSizeModifier * ratio);

            yield return new WaitForEndOfFrame();
        }

        Visual.transform.localPosition = new Vector3(
                Visual.transform.localPosition.x,
                Visual.transform.localPosition.y,
                m_initialPositionZ);
        Visual.transform.localScale = m_initialScale;
    }
}
