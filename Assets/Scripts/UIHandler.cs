using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private VisualElement m_fireBallTimer;
    public static UIHandler instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        m_fireBallTimer = uiDocument.rootVisualElement.Q<VisualElement>("FireballBar");
        SetHealthValue(1.0f);
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    public void fireballTimer(float cooldown)
    {
        m_fireBallTimer.style.width = Length.Percent(0f);
        StartCoroutine(FillFireballTimer(cooldown));
    }
    private IEnumerator FillFireballTimer(float cooldown)
    {
        float startTime = Time.time;
        float endTime = startTime + cooldown;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float newPercent = elapsedTime / cooldown;
            m_fireBallTimer.style.width = Length.Percent(100 * newPercent);
            yield return null;
        }

        m_fireBallTimer.style.width = Length.Percent(100);
    }
}
