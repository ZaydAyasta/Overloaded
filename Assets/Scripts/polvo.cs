using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Polvo : MonoBehaviour
{
    [Header("Limpieza")]
    [Tooltip("Velocidad base de limpieza por segundo (alpha por segundo).")]
    public float cleanRate = 0.75f;

    [Tooltip("Alpha por debajo del que consideramos 'limpio'.")]
    public float cleanedThreshold = 0.05f;

    private SpriteRenderer sr;
    private Collider2D col;
    private bool cleaned = false;

    // Referencia opcional al manager (si no se asigna, lo busca)
    public CleaningManager manager;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // importante: el área de barrido es trigger
    }

    void Start()
    {
        if (manager == null) manager = FindFirstObjectByType<CleaningManager>();
        // registra este polvo en el manager
        manager?.RegisterPolvo(this);
    }

    /// <summary>
    /// Aplica limpieza (0..infinito). normalizado por deltaTime ya viene desde la escoba.
    /// </summary>
    public void ApplySweep(float strength)
    {
        if (cleaned || sr == null) return;

        Color c = sr.color;
        c.a = Mathf.Clamp01(c.a - strength);
        sr.color = c;

        if (c.a <= cleanedThreshold)
        {
            cleaned = true;
            sr.enabled = false;
            col.enabled = false;
            manager?.OnPolvoCleaned(this);
        }
    }

    // Para reinicios / test
    public void ResetPolvo(float alpha = 1f)
    {
        cleaned = false;
        if (sr != null)
        {
            var c = sr.color; c.a = Mathf.Clamp01(alpha); sr.color = c;
            sr.enabled = true;
        }
        if (col != null) col.enabled = true;
    }
}
