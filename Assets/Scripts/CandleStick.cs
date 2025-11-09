using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Candlestick : MonoBehaviour
{
    [Header("Estado inicial")]
    public bool startLit = false;

    [Header("Animator")]
    public Animator anim;                  // asignar en inspector
    public string litParam = "Lit";        // BOOL
    public string flashTrigger = "Flash"; 
    public bool useFlashTrigger = false;

    [Header("Flash visual")]
    public Color flashColor = new Color(1f, 0.95f, 0.6f, 1f);
    public float flashDuration = 0.15f;
    public float returnBlend = 0.08f;
    public float flashCooldown = 0.2f;

    [Header("Manager")]
    public CandleManager manager;

    [Header("Debug")]
    public bool debugLogs = false;

    public bool IsLit { get; private set; }
    public bool IsFlashing { get; private set; }

    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashCo;
    private float nextFlashTime = 0f;

    void Awake()
    {
        // sprite
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);

        // animator
        if (anim == null)
            anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);

        if (sr != null)
            originalColor = sr.color;
    }

    void Start()
    {
        // autowire manager
#if UNITY_2023_1_OR_NEWER
        if (manager == null) manager = FindFirstObjectByType<CandleManager>(FindObjectsInactive.Include);
#else
        if (manager == null) manager = FindObjectOfType<CandleManager>();
#endif
        manager?.RegisterCandlestick(this);

        // estado inicial sin notificar
        SetLit(startLit, false);
    }

    [ContextMenu("TEST/LightUp")]
    public void LightUp_TEST() => LightUp();

    public void LightUp()
    {
        if (IsLit) { TriggerFlash(); return; }
        SetLit(true);
        TriggerFlash();
    }

    public void Toggle()
    {
        SetLit(!IsLit);
        TriggerFlash();
    }

    public void SetLit(bool value, bool notifyManager = true)
    {
        IsLit = value;

        // cambiar anim
        if (anim != null)
        {
            foreach (var p in anim.parameters)
                if (p.name == litParam && p.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(litParam, value);
                    break;
                }
        }

        // manager
        if (notifyManager && manager != null)
        {
            if (value) manager.OnCandlestickLit(this);
            else       manager.OnCandlestickUnlit(this);
        }
    }

    public void TriggerFlash()
    {
        if (Time.unscaledTime < nextFlashTime) return;

        // anim trigger
        if (useFlashTrigger && anim != null)
        {
            foreach (var p in anim.parameters)
                if (p.name == flashTrigger && p.type == AnimatorControllerParameterType.Trigger)
                    anim.SetTrigger(flashTrigger);
        }

        // color flash
        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRoutine());

        nextFlashTime = Time.unscaledTime + flashCooldown;
    }

    IEnumerator FlashRoutine()
    {
        IsFlashing = true;

        if (sr != null)
        {
            sr.color = flashColor;

            float end = Time.unscaledTime + flashDuration;
            while (Time.unscaledTime < end) yield return null;

            float t = 0f;
            while (t < returnBlend)
            {
                t += Time.unscaledDeltaTime;
                sr.color = Color.Lerp(flashColor, originalColor, t / returnBlend);
                yield return null;
            }
            sr.color = originalColor;
        }

        IsFlashing = false;
        flashCo = null;
    }
}
