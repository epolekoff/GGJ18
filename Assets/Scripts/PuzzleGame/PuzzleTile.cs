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
    public GameObject OffscreenOverlay;
    public AnimationCurve FallCurve;

    public PuzzleTileType PuzzleTileType = PuzzleTileType.None;

    public bool CanBeMatched { get { return !IsFalling && !IsOffscreen && !IsTransmitting; } }
    public bool IsFalling = false;
    public bool IsOffscreen
    {
        get
        {
            return m_isOffscreen;
        }
        set
        {
            if(OffscreenOverlay != null)
            {
                OffscreenOverlay.SetActive(value);
            }
            m_isOffscreen = value;
        }
    }
    private bool m_isOffscreen;

    public bool IsTransmitting
    {
        get
        {
            return m_isTransmitting;
        }
        set
        {
            if (OffscreenOverlay != null)
            {
                OffscreenOverlay.SetActive(value);
            }
            m_isTransmitting = value;
        }
    }
    private bool m_isTransmitting;

    public Vector3 TargetFallingLocalPosition { get; set; }

    private const float SelectedGrowSpeed = 0.2f;
    private const float SelectedShrinkSpeed = 0.1f;
    private const float SelectedPositionZ = 10f;
    private const float SelectedSizeModifier = 0.1f;
    private const float FallTime = 0.5f;
    private Coroutine m_fallingCoroutine;

    private Vector3 m_initialScale;
    private float m_initialPositionZ;
    private Coroutine m_growCoroutine;
    private Coroutine m_shrinkCoroutine;
    private bool m_isGrown = false;

    // Use this for initialization
    void Start () {
        m_initialPositionZ = transform.localPosition.z;
        m_initialScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp()
    {
        if (m_isGrown)
        {
            m_shrinkCoroutine = StartCoroutine(ShrinkVisual());
            PuzzleManager.Instance.EnableDragCursor(false);
            PuzzleManager.Instance.SwapWithCurrentHoveredTile(this);
        }
    }

    void OnMouseDown()
    {
        if(PuzzleManager.Instance.GameActive && CanBeMatched)
        {
            m_growCoroutine = StartCoroutine(GrowVisual());
            PuzzleManager.Instance.EnableDragCursor(true);
        }
    }

    private IEnumerator GrowVisual()
    {
        m_isGrown = true;
        float timer = SelectedGrowSpeed;
        while(timer > 0)
        {
            timer -= Time.deltaTime;

            float ratio = 1-(timer / SelectedGrowSpeed);

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
        float timer = SelectedShrinkSpeed;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            float ratio = timer / SelectedShrinkSpeed;

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
        m_isGrown = false;
    }

    public void FallIntoPosition(PuzzleManager.OnTileFallComplete callback, int comboDepth)
    {
        if (m_fallingCoroutine != null)
        {
            StopCoroutine(m_fallingCoroutine);
        }

        IsFalling = true;
        m_fallingCoroutine = StartCoroutine(FallIntoPositionCoroutine(callback, comboDepth));
    }

    public IEnumerator FallIntoPositionCoroutine(PuzzleManager.OnTileFallComplete callback, int comboDepth)
    {
        float timer = 0;

        Vector3 startPosition = transform.localPosition;

        while(timer < FallTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / FallTime;

            float yRatio = (transform.localPosition.y - startPosition.y) / (TargetFallingLocalPosition.y - startPosition.y);
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                FallCurve.Evaluate(ratio) * (TargetFallingLocalPosition.y - startPosition.y) + startPosition.y, 
                transform.localPosition.z);

            yield return new WaitForEndOfFrame();
        }

        IsFalling = false;
        m_fallingCoroutine = null;
        callback(this, comboDepth);
    }
}
