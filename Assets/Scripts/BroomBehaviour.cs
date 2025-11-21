using UnityEngine;

public class BroomBehaviour : GrabbableBehaviour
{
    [Header("Oscilación")]
    public float rotationAmplitude = 15f;   // grados
    public float rotationSpeed = 6f;        // oscilaciones por segundo aprox.

    [Header("Barrido")]
    [Tooltip("Collider (2D) trigger que representa el área donde barre la escoba.")]
    public Collider2D sweepAreaTrigger;

    [Tooltip("Fuerza de limpieza por segundo aplicada a los polvos dentro del área.")]
    public float sweepStrengthPerSecond = 1.2f;

    [Header("FX de polvo")]
    [SerializeField] private ParticleSystem polvoParticle;

    [Header("Audio")]
    [Tooltip("Sonido de barrer (loop mientras se está barriendo).")]
    public AudioSource sweepAudio;
    [Range(0f, 1f)]
    public float sweepAudioVolume = 1f;     // AUDIO

    private float angle = 0f;
    private float tOsc = 0f;
    private Transform self;

    private bool polvoDentro = false;
    Transform _particlePolvo;

    void Awake()
    {
        self = transform;
        if (sweepAreaTrigger != null) sweepAreaTrigger.isTrigger = true;

        if (polvoParticle != null) _particlePolvo = polvoParticle.transform;

        // AUDIO: configurar volumen y loop (opcional)
        if (sweepAudio != null)
        {
            sweepAudio.loop = true;
            sweepAudio.volume = sweepAudioVolume;
        }
    }

    public override void OnPickedUp(PlayerGrab player)
    {
        // opcional: reset visual
        self.localRotation = Quaternion.identity;
        SetSweepAreaActive(false);

        // AUDIO: por si estaba sonando en el suelo
        if (sweepAudio != null && sweepAudio.isPlaying)
            sweepAudio.Stop();
    }

    public override void OnDropped(PlayerGrab player)
    {
        // vuelve neutro
        self.localRotation = Quaternion.identity;
        SetSweepAreaActive(false);

        // AUDIO
        if (sweepAudio != null && sweepAudio.isPlaying)
            sweepAudio.Stop();
    }

    public override void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput)
    {
        bool moving = Mathf.Abs(moveInput.x) > 0.01f || Mathf.Abs(moveInput.y) > 0.01f;

        // Oscilación
        if (moving)
        {
            tOsc += Time.deltaTime * rotationSpeed;
            float target = Mathf.Sin(tOsc) * rotationAmplitude;
            angle = Mathf.Lerp(angle, target, Time.deltaTime * 10f);
        }
        else
        {
            angle = Mathf.Lerp(angle, 0f, Time.deltaTime * 10f);
        }
        self.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Área de barrido activa solo si nos movemos
        SetSweepAreaActive(moving);

        if (!moving || sweepAreaTrigger == null)
        {
            polvoDentro = false;
            SafeParticle(p => { p.Stop(); });

            // AUDIO: si no se mueve, parar sonido
            if (sweepAudio != null && sweepAudio.isPlaying)
                sweepAudio.Stop();

            return;
        }

        // Aplica limpieza a todos los polvos que toquen el área
        var filter = new ContactFilter2D { useTriggers = true, useLayerMask = false };
        Collider2D[] hits = new Collider2D[16];
        int count = sweepAreaTrigger.Overlap(filter, hits);
        float amount = sweepStrengthPerSecond * Time.deltaTime;
        polvoDentro = false;

        for (int i = 0; i < count; i++)
        {
            var h = hits[i];
            if (h == null) continue;

            // si el collider pertenece a un Polvo
            var p = h.GetComponent<Polvo>();
            if (p == null) p = h.GetComponentInParent<Polvo>();

            if (p != null)
            {
                p.ApplySweep(amount);
                polvoDentro = true;

                if (polvoParticle != null)
                    _particlePolvo.position = h.transform.position;
            }
        }

        if (polvoDentro)
        {
            SafeParticle(p => { if (!p.isPlaying) p.Play(); });

            // AUDIO: si está barriendo polvo, asegurar que suene
            if (sweepAudio != null && !sweepAudio.isPlaying)
                sweepAudio.Play();
        }
        else
        {
            SafeParticle(p => { if (p.isPlaying) p.Stop(); });

            // AUDIO: sin polvo, parar sonido
            if (sweepAudio != null && sweepAudio.isPlaying)
                sweepAudio.Stop();
        }
    }

    private void SetSweepAreaActive(bool active)
    {
        if (sweepAreaTrigger == null) return;
        sweepAreaTrigger.enabled = active;
    }

    void SafeParticle(System.Action<ParticleSystem> action)
    {
        if (polvoParticle == null || polvoParticle.Equals(null)) return;
        action(polvoParticle);
    }
}
