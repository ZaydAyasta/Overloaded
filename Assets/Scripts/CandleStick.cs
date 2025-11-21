using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Candlestick : MonoBehaviour
{
    [Header("Estado inicial")]
    public bool startLit = false;

    [Header("Animator")]
    public Animator anim;
    public string litParam = "Lit";
    public string flashTrigger = "Flash";
    public bool useFlashTrigger = false;

    [Header("Flash visual")]
    public Color flashColor = new Color(1f, 0.95f, 0.6f, 1f);
    public float flashDuration = 0.15f;
    public float returnBlend = 0.08f;
    public float flashCooldown = 0.2f;

    [Header("Manager")]
    public CandleManager manager;

    [Header("Sonido (opcional)")]
    public AudioSource audioSource;    // arrastra uno aquí
    public AudioClip lightSound;       // sonido al encender
    [Range(0f, 1f)]
    public float soundVolume = 1f;

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
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);

        if (anim == null)
            anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>(true);

        if (sr != null)
            originalColor = sr.color;

        // Asignar automáticamente AudioSource si no está puesto
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
#if UNITY_2023_1_OR_NEWER
        if (manager == null) manager = FindFirstObjectByType<CandleManager>(FindObjectsInactive.Include);
#else
        if (manager == null) manager = FindObjectOfType<CandleManager>();
#endif
        manager?.RegisterCandlestick(this);

        SetLit(startLit, false);
    }

    [ContextMenu("TEST/LightUp")]
    public void LightUp_TEST() => LightUp();

    public void LightUp()
    {
        if (IsLit)
        {
            TriggerFlash();
            return;
        }

        SetLit(true);
        PlayLightSound();   // <<< 🎵 Sonido
        TriggerFlash();
    }

    public void Toggle()
    {
        SetLit(!IsLit);

        if (IsLit)
            PlayLightSound();  // <<< 🎵 Sonido sólo al encender

        TriggerFlash();
    }

    public void SetLit(bool value, bool notifyManager = true)
    {
        IsLit = value;

        if (anim != null)
        {
            foreach (var p in anim.parameters)
                if (p.name == litParam && p.type == AnimatorControllerParameterType.Bool)
                {
                    anim.SetBool(litParam, value);
                    break;
                }
        }

        if (notifyManager && manager != null)
        {
            if (value) manager.OnCandlestickLit(this);
            else manager.OnCandlestickUnlit(this);
        }
    }

    public void TriggerFlash()
    {
        if (Time.unscaledTime < nextFlashTime) return;

        if (useFlashTrigger && anim != null)
        {
            foreach (var p in anim.parameters)
                if (p.name == flashTrigger && p.type == AnimatorControllerParameterType.Trigger)
                    anim.SetTrigger(flashTrigger);
        }

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRoutine());

        nextFlashTime = Time.unscaledTime + flashCooldown;
    }

    void PlayLightSound()
    {
        if (audioSource != null && lightSound != null)
        {
            audioSource.PlayOneShot(lightSound, soundVolume);
        }
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
