using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Clothesline : MonoBehaviour
{
    [Header("Duración de colgado")]
    public float hangDuration = 1.4f;

    [Header("Sprites del tendedero")]
    public Sprite idleSprite;      // sin ropa
    public Sprite withClothes;     // con ropa (estado final)

    [Header("FX de humo")]
    public Transform smokeRoot;        
    public float smokeScaleMin = 0.8f;
    public float smokeScaleMax = 1.25f;
    public float smokePulseSpeed = 6f;         
    public float smokeSnapRotInterval = 0.08f; 
    public Vector2 smokeSnapAngles = new Vector2(-25f, 25f);

    [Header("Audio (opcional)")]
    public AudioSource sfxStart;
    public AudioSource sfxDone;

    [Header("Bloqueo de anim del Player (opcional)")]
    public string playerCleanTrigger = ""; 

    [Header("Manager")]
    public ClotheslineManager manager;

    // --- Estado ---
    public bool IsBusy { get; private set; }
    public bool IsCompleted { get; private set; }

    private SpriteRenderer sr;
    private Coroutine workCo;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Asegurar trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (smokeRoot != null)
            smokeRoot.gameObject.SetActive(false);

        if (idleSprite != null)
            sr.sprite = idleSprite;
    }

    void Start()
    {
        // Registro automático en el manager
#if UNITY_2023_1_OR_NEWER
        if (manager == null) manager = FindFirstObjectByType<ClotheslineManager>(FindObjectsInactive.Include);
#else
        if (manager == null) manager = FindObjectOfType<ClotheslineManager>();
#endif
        manager?.RegisterClothesline(this, IsCompleted);
    }

    void OnDestroy()
    {
        manager?.UnregisterClothesline(this);
    }

    // =====================================================================
    //                          API PUBLICA
    // =====================================================================

    /// <summary>
    /// Llamado por BasketBehaviour cuando el jugador presiona E cerca.
    /// </summary>
    public void TryHang(PlayerGrab player, BasketBehaviour basket)
    {
        if (basket == null || basket.IsEmpty) return;
        if (IsBusy || IsCompleted) return;

        if (workCo != null) StopCoroutine(workCo);
        workCo = StartCoroutine(HangRoutine(player, basket));
    }

    // =====================================================================
    //                           RUTINA PRINCIPAL
    // =====================================================================

    IEnumerator HangRoutine(PlayerGrab player, BasketBehaviour basket)
    {
        IsBusy = true;

        // Bloqueo de movimiento del jugador
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        Animator panim = player.GetComponent<Animator>();

        if (pm != null) pm.enabled = false;
        if (panim != null && !string.IsNullOrEmpty(playerCleanTrigger))
            panim.SetTrigger(playerCleanTrigger);

        // SFX inicio
        if (sfxStart != null) sfxStart.Play();

        // Encender humo
        if (smokeRoot != null)
        {
            smokeRoot.gameObject.SetActive(true);
            StartCoroutine(SmokeRoutine(smokeRoot));
        }

        // Esperar duración completa
        float end = Time.time + hangDuration;
        while (Time.time < end) yield return null;

        // Apagar humo
        if (smokeRoot != null) smokeRoot.gameObject.SetActive(false);

        // Cambiar sprite a "con ropa"
        if (sr != null && withClothes != null)
            sr.sprite = withClothes;

        // Marcar estados
        IsBusy = false;
        IsCompleted = true;

        // Notificar manager
        manager?.OnClotheslineCompleted(this);

        // Vaciar cesta y bloquearla
        basket.MarkEmptiedAndDisable();

        // SFX fin
        if (sfxDone != null) sfxDone.Play();

        // Liberar movimiento
        if (pm != null) pm.enabled = true;

        workCo = null;
    }


    // =====================================================================
    //                        FX HUMO: ESCALA + ROTACIÓN
    // =====================================================================

    IEnumerator SmokeRoutine(Transform t)
    {
        float snapTimer = 0f;
        while (t.gameObject.activeSelf)
        {
            // pulso de escala tipo “respiración”
            float scale = Mathf.Lerp(
                smokeScaleMin,
                smokeScaleMax,
                0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.PI * 2f * smokePulseSpeed)
            );
            t.localScale = new Vector3(scale, scale, 1f);

            // rotaciones bruscas
            snapTimer -= Time.deltaTime;
            if (snapTimer <= 0f)
            {
                float ang = Random.Range(smokeSnapAngles.x, smokeSnapAngles.y);
                t.localRotation = Quaternion.Euler(0, 0, ang);
                snapTimer = smokeSnapRotInterval;
            }

            yield return null;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f, new Vector3(1.2f, 0.6f, 1f));
    }
#endif
}
