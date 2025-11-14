using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimerGame : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] float startTime = 120f;   // segundos iniciales

    [Header("UI References")]
    [SerializeField] TMP_Text timerText;

    float remaining;
    bool isRunning;

    [SerializeField] GameObject _prefab;

    void Awake()
    {
        remaining = startTime;
        UpdateDisplay();
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        remaining -= Time.deltaTime;
        remaining = Mathf.Max(remaining, 0f);

        UpdateDisplay();

        

        if (remaining <= 0f)
        {
            isRunning = false;
            GameEnd.Instance.Lose();
        }
            

        
    }

    void UpdateDisplay()
    {
        int min = (int)(remaining / 60);
        int sec = (int)(remaining % 60);

        timerText.text = $"{min:00}:{sec:00}";
    }
    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public bool GetRunning () { return isRunning; }

    public void ResetTimer(float newStart)
    {
        startTime = newStart;
        remaining = newStart;
        UpdateDisplay();
    }

    
}
