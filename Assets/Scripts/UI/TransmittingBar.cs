using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransmittingBar : MonoBehaviour {

    public Image BarFill;

    private float m_animationLifetime = 1f;
    private float m_loadingLifetime = 1f;


    /// <summary>
    /// Set the lifetime of this object
    /// </summary>
    /// <param name="lifetime"></param>
    public void SetLifetime(float lifetime)
    {
        m_animationLifetime = lifetime / 6;
        m_loadingLifetime = 4 * (lifetime / 6);
        StartCoroutine(StartupTime());
    }

    /// <summary>
    /// Give time for the animation to play.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartupTime()
    {
        float timer = 0;
        while(timer < m_animationLifetime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        StartCoroutine(LoadingTime());
    }

    /// <summary>
    /// Fill the bar.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadingTime()
    {
        float timer = 0;
        while (timer < m_loadingLifetime)
        {
            timer += Time.deltaTime;

            // Fill the bar
            float ratio = (timer / m_loadingLifetime);
            BarFill.fillAmount = ratio;

            yield return null;
        }
        
        StartCoroutine(ClosingTime());
    }

    /// <summary>
    /// Give it time to close out.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClosingTime()
    {
        GetComponent<Animator>().SetTrigger("Hide");

        float timer = 0;
        while (timer < m_animationLifetime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Print the line
        GameManager.Instance.PhoneCanvas.TerminalWindowText.TriggerTransmissionComplete();

        Destroy(gameObject);
    }
}
