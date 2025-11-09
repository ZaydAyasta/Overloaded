using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class WindowPanel : MonoBehaviour
{
    [Header("Limpieza")]
    public float cleanNeeded = 3f;

    [Header("Animación")]
    public Animator anim;                     // Asignar en Inspector (mismo GO o hijo)
    public string cleanedParam = "Cleaned";   // Bool o Trigger en el Animator
    public bool useTrigger = false;           // true = Trigger, false = Bool

    [Header("Feedback")]
    public float flashDuration = 0.15f;
    public Color flashColor = new Color(0.9f, 0.9f, 1f, 1f);

    [Header("Manager (opcional si lo arrastras)")]
    public WindowManager manager;

    private float cleanGot = 0f;
    private bool cleaned = false;

    private SpriteRenderer sr;
    private Collider2D col;
    private Color originalColor;

    public bool IsCleaned => cleaned;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (anim == null) anim = GetComponent<Animator>();
        originalColor = sr.color;
    }

    void Start()
    {
        if (manager == null) manager = FindFirstObjectByType<WindowManager>();
        manager?.RegisterWindow(this);
    }

    public void ApplyScrub(float amount)
    {
        if (cleaned) return;
        if (amount <= 0f) return;

        cleanGot += amount;

        if (cleanGot >= cleanNeeded)
        {
            cleaned = true;

            if (anim != null)
            {
                if (useTrigger) anim.SetTrigger(cleanedParam);
                else            anim.SetBool(cleanedParam, true);
            }

            StartCoroutine(FlashWhite());
            manager?.OnWindowCleaned(this);
        }
    }

    IEnumerator FlashWhite()
    {
        Color prev = sr.color;
        sr.color = flashColor;

        float end = Time.unscaledTime + flashDuration;
        while (Time.unscaledTime < end) yield return null;

        // pequeño blend de regreso
        float blend = 0.07f;
        float e = 0f;
        while (e < blend)
        {
            e += Time.unscaledDeltaTime;
            sr.color = Color.Lerp(flashColor, prev, Mathf.Clamp01(e / blend));
            yield return null;
        }
        sr.color = prev;
    }
}
