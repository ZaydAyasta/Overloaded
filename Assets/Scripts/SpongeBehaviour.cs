using UnityEngine;
using System.Collections.Generic;

public class SpongeBehaviour : GrabbableBehaviour
{
    [Header("Interacción")]
    public KeyCode cleanKey = KeyCode.E;

    [Header("Oscilación (tipo escoba)")]
    public float rotationAmplitude = 15f;   // grados del vaivén
    public float rotationSpeed = 6f;        // rapidez del vaivén
    public float smooth = 10f;              // suavizado

    [Header("Área de fregado")]
    [Tooltip("Trigger 2D que define el área donde la esponja limpia ventanas.")]
    public Collider2D wipeAreaTrigger;
    [Tooltip("Capas consideradas 'ventana'. Esos objetos deben tener WindowPanel en el GO o en el padre.")]
    public LayerMask windowMask;

    [Header("Limpieza")]
    public float cleanPerSecond = 1.6f;     // progreso por segundo al mantener E

    [Header("FX (opcionales)")]
    public ParticleSystem scrubFx;
    public AudioSource scrubSfx;

    // ---- Estado
    private Transform self;
    private float tOsc = 0f;
    private float angle = 0f;
    private bool nearWindow = false;
    private bool isCleaning = false;

    public override bool WantsToBlockDrop() => nearWindow || isCleaning;
    public override string GetDropBlockHint() =>
        (nearWindow || isCleaning) ? "No puedes soltar mientras limpias la ventana." : null;

    void Awake()
    {
        self = transform;
        if (wipeAreaTrigger != null)
        {
            wipeAreaTrigger.isTrigger = true;
            wipeAreaTrigger.enabled  = true; // SIEMPRE activo para detectar
        }
    }

    public override void OnPickedUp(PlayerGrab player)
    {
        angle = 0f; tOsc = 0f;
        self.localRotation = Quaternion.identity;
        StopFx();
    }

    public override void OnDropped(PlayerGrab player)
    {
        angle = 0f; tOsc = 0f;
        self.localRotation = Quaternion.identity;
        StopFx();
    }

    public override void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput)
    {
        bool keyHeld = Input.GetKey(cleanKey);

        // 1) Detectar ventanas dentro del área
        WindowPanel[] windows = GatherWindowsInArea();
        nearWindow = windows.Length > 0;

        // 2) SOLO limpiamos y oscilamos si hay ventana y E está presionada
        isCleaning = keyHeld && nearWindow;

        // 3) Oscilación
        if (isCleaning)
        {
            tOsc += Time.deltaTime * rotationSpeed;
            float target = Mathf.Sin(tOsc) * rotationAmplitude;
            angle = Mathf.Lerp(angle, target, Time.deltaTime * smooth);
        }
        else
        {
            angle = Mathf.Lerp(angle, 0f, Time.deltaTime * smooth);
        }
        self.localRotation = Quaternion.Euler(0f, 0f, angle);

        // 4) Aplicar limpieza SOLO si isCleaning
        if (isCleaning)
        {
            float amount = cleanPerSecond * Time.deltaTime;
            for (int i = 0; i < windows.Length; i++)
                windows[i]?.ApplyScrub(amount);

            PlayFx();
        }
        else
        {
            StopFx();
        }
    }

    private WindowPanel[] GatherWindowsInArea()
    {
        if (wipeAreaTrigger == null) return System.Array.Empty<WindowPanel>();

        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = windowMask
        };

        Collider2D[] hits = new Collider2D[16];
        int count = wipeAreaTrigger.Overlap(filter, hits);
        if (count <= 0) return System.Array.Empty<WindowPanel>();

        var list = new List<WindowPanel>(count);
        for (int i = 0; i < count; i++)
        {
            var c = hits[i];
            if (!c) continue;
            var w = c.GetComponent<WindowPanel>() ?? c.GetComponentInParent<WindowPanel>();
            if (w != null && !list.Contains(w)) list.Add(w);
        }
        return list.ToArray();
    }

    private void PlayFx()
    {
        if (scrubFx != null && !scrubFx.isPlaying) scrubFx.Play();
        if (scrubSfx != null && !scrubSfx.isPlaying) scrubSfx.Play();
    }

    private void StopFx()
    {
        if (scrubFx != null && scrubFx.isPlaying) scrubFx.Stop();
        if (scrubSfx != null && scrubSfx.isPlaying) scrubSfx.Stop();
    }
}
