using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class TableTask : MonoBehaviour
{
    [Header("Interacción")]
    public KeyCode interactKey = KeyCode.E;
    public Collider2D interactTrigger;         
    public LayerMask playerMask;

    [Header("Duración del proceso")]
    public float cleanDuration = 1.4f;

    [Header("Animación del jugador (opcional)")]
    public string playerCleanTrigger = "";      

    [Header("Humo FX (Woo)")]
    public Transform smokeRoot;        
    public float smokeScaleMin = 0.85f;
    public float smokeScaleMax = 1.25f;
    public float smokePulseSpeed = 6f;         
    public float smokeSnapRotInterval = 0.08f; 
    public Vector2 smokeSnapAngles = new Vector2(-25f, 25f);

    [Header("Audio (opcional)")]
    public AudioSource sfxStart;
    public AudioSource sfxDone;

    [Header("Sprite final")]
    public Sprite sprCleanTable;

    [Header("Manager")]
    public TableManager manager;

    // --- estado ---
    private bool isDone = false;
    private bool isBusy = false;

    private SpriteRenderer sr;

    void Awake()
    {
        if (interactTrigger != null)
        {
            interactTrigger.isTrigger = true;
            interactTrigger.enabled = true;
        }

        sr = GetComponent<SpriteRenderer>();

        if (smokeRoot != null)
            smokeRoot.gameObject.SetActive(false);
    }

    void Start()
    {
        if (manager == null) manager = FindFirstObjectByType<TableManager>();
        manager?.RegisterTable(this);
    }

    void Update()
    {
        if (isDone || isBusy)
            return;

        Transform p = DetectPlayer();
        if (p == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartCoroutine(CleanRoutine(p));
        }
    }

    IEnumerator CleanRoutine(Transform playerT)
    {
        isBusy = true;

        // bloquear movimiento del jugador
        PlayerMovement pm = playerT.GetComponent<PlayerMovement>();
        Animator pAnim = playerT.GetComponent<Animator>();

        if (pm) pm.enabled = false;

        if (pAnim && !string.IsNullOrEmpty(playerCleanTrigger))
            pAnim.SetTrigger(playerCleanTrigger);

        if (sfxStart) sfxStart.Play();

        if (smokeRoot != null)
        {
            smokeRoot.gameObject.SetActive(true);
            StartCoroutine(SmokeRoutine(smokeRoot));
        }

        float end = Time.time + cleanDuration;
        while (Time.time < end)
            yield return null;

        if (smokeRoot != null)
            smokeRoot.gameObject.SetActive(false);

        if (sr && sprCleanTable)
            sr.sprite = sprCleanTable;

        isDone = true;
        manager?.OnTableCleaned(this);

        if (sfxDone) sfxDone.Play();

        if (pm) pm.enabled = true;

        if (interactTrigger) interactTrigger.enabled = false;

        isBusy = false;
    }

    Transform DetectPlayer()
    {
        if (!interactTrigger) return null;

        ContactFilter2D filter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = playerMask,
            useTriggers = true
        };

        Collider2D[] hits = new Collider2D[8];
        int count = interactTrigger.Overlap(filter, hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] == null) continue;

            PlayerGrab pg = hits[i].GetComponent<PlayerGrab>();
            if (pg != null)
                return pg.transform;
        }

        return null;
    }

    IEnumerator SmokeRoutine(Transform t)
    {
        float snapTimer = 0f;

        while (t.gameObject.activeSelf)
        {
            float s = Mathf.Lerp(
                smokeScaleMin, 
                smokeScaleMax, 
                0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.PI * 2f * smokePulseSpeed)
            );
            t.localScale = new Vector3(s, s, 1f);

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

    public bool IsDone => isDone;
    public bool IsBusy => isBusy;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (interactTrigger == null) return;
        Gizmos.color = Color.magenta;
        var b = interactTrigger.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
#endif
}
