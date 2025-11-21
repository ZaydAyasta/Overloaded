using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Plant : MonoBehaviour
{
    [Header("Riego")]
    public float waterNeeded = 3f;

    [Header("Animación")]
    public Animator anim;                         // Asignado en Inspector
    public string completedParam = "Completed";   // Bool en el Animator

    [Header("Sprites")]
    public Sprite sprNormal;      // sprite de planta antes de regar
    public Sprite sprCompleted;   // sprite de planta ya regada / crecida

    [Header("Feedback")]
    public float flashDuration = 0.2f;            // Parpadeo blanco rápido al terminar

    private float waterGot = 0f;
    private bool completed = false;

    private SpriteRenderer sr;
    private Collider2D col;
    private Color originalColor;

    public GardenManager manager;

    public bool IsCompleted => completed;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = false;
        originalColor = sr.color;

        if (anim == null) anim = GetComponent<Animator>();

        // Sprite inicial de la planta
        if (sprNormal != null)
            sr.sprite = sprNormal;
    }

    void Start()
    {
        if (manager == null) manager = FindFirstObjectByType<GardenManager>();
        manager?.RegisterPlant(this);
    }

    public void ApplyWater(float amount)
    {
        if (completed) return;

        waterGot += Mathf.Max(0f, amount);

        if (waterGot >= waterNeeded)
        {
            completed = true;

            // Cambiar sprite a planta terminada
            if (sr != null && sprCompleted != null)
                sr.sprite = sprCompleted;

            // Animación
            if (anim != null)
                anim.SetBool(completedParam, true);

            // Efecto de parpadeo
            StartCoroutine(FlashWhite());

            // Avisar al manager
            manager?.OnPlantCompleted(this);
        }
    }

    IEnumerator FlashWhite()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
