using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerGrab : MonoBehaviour
{
    [Header("Zona de agarre (edítala en el editor)")]
    [Tooltip("BoxCollider2D que define el área para agarrar (puede estar en un hijo).")]
    public BoxCollider2D grabBox;

    [Header("Filtro de detección")]
    [Tooltip("Selecciona la capa 'Objects'.")]
    public LayerMask grabbableMask;

    [Header("Control")]
    public KeyCode grabKey = KeyCode.E;

    private Animator anim;
    private bool isCarrying;

    // Estado actual
    private Transform carriedT;
    private Rigidbody2D carriedRb;
    private Grabbable carriedG;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); 
        if (grabBox == null)
            Debug.LogWarning("PlayerGrab: asigna un BoxCollider2D a 'grabBox'.");
    }

    void Update()
{
    if (Input.GetKeyDown(grabKey))
    {
        if (carriedT == null) TryGrab();
        else Drop();
    }

    // Si estamos llevando algo, actualizar su posición y flip visual
    if (carriedT != null && carriedG != null)
    {
        float sign = (carriedG.mirrorOffsetWithPlayerFlip && sr != null && sr.flipX) ? -1f : 1f;
        Vector3 p = transform.position;
        Vector2 off = carriedG.carryOffset;

        carriedT.position = new Vector3(
            p.x + off.x * sign,
            p.y + off.y,
            carriedT.position.z
        );

        // 🔁 VOLTEAR EL SPRITE SI EL PLAYER SE DA VUELTA
        var objSprite = carriedT.GetComponent<SpriteRenderer>();
        if (objSprite != null)
            objSprite.flipX = sr.flipX;  // mismo flip que el jugador
    }

    if (carriedT != null && carriedG != null)
{
    var behaviour = carriedT.GetComponent<GrabbableBehaviour>();
    if (behaviour != null)
    {
        // Le pasamos el movimiento actual (solo horizontal, si quieres)
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        behaviour.OnCarriedUpdate(this, moveInput);
    }
}
}


    // ---------- AGARRAR ----------
    void TryGrab()
    {
        if (grabBox == null) return;

        // Usa el collider real como volumen de consulta
        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.useTriggers = true;               // por si algún objeto accidentalmente es trigger
        filter.SetLayerMask(grabbableMask);      // incluye 'Objects'

        Collider2D[] hits = new Collider2D[16];
        int count = grabBox.Overlap(filter, hits);
        if (count <= 0) return;

        // Elige el más cercano al centro de la caja con Tag "Grabbable"
        Vector2 center = grabBox.bounds.center;
        Collider2D best = null; float bestD = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var h = hits[i];
            if (h == null) continue;
            if (!h.CompareTag("Grabbable")) continue;

            float d = ((Vector2)center - h.ClosestPoint(center)).sqrMagnitude;
            if (d < bestD) { bestD = d; best = h; }
        }
        if (best == null) return;

        carriedT  = best.transform;
        carriedRb = best.attachedRigidbody ?? best.GetComponent<Rigidbody2D>();
        carriedG  = best.GetComponent<Grabbable>();
        if (carriedRb == null || carriedG == null) { carriedT = null; carriedG = null; return; }

        // Pausar física mientras se lleva
        carriedRb.bodyType = RigidbodyType2D.Kinematic;
        carriedRb.linearVelocity = Vector2.zero;
        carriedRb.angularVelocity = 0f;
        carriedRb.freezeRotation = true;
        carriedRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        carriedG.OnPickedUp();

        // Colocar una primera vez usando su offset (luego se mantiene en Update)
        float sign = (carriedG.mirrorOffsetWithPlayerFlip && sr != null && sr.flipX) ? -1f : 1f;
        Vector3 p2 = transform.position;
        Vector2 off2 = carriedG.carryOffset;
        carriedT.position = new Vector3(p2.x + off2.x * sign, p2.y + off2.y, carriedT.position.z);

        isCarrying = true;
        if (anim != null)
            anim.SetBool("IsCarrying", true);
    }

    // ---------- SOLTAR ----------
    void Drop()
    {
        if (carriedT == null) return;

        carriedRb.bodyType = RigidbodyType2D.Dynamic;
        carriedRb.freezeRotation = false;
        carriedRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        carriedG.OnDropped();

        carriedT  = null;
        carriedRb = null;
        carriedG  = null;

        isCarrying = false;
        if (anim != null)
            anim.SetBool("IsCarrying", false);
    }

    // ---------- Gizmo: la caja se pinta cian (verde si hay algo Grabbable dentro) ----------
    void OnDrawGizmosSelected()
    {
        if (grabBox == null) return;

        Color c = Color.cyan;
        try
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.useLayerMask = true;
            filter.useTriggers  = true;
            filter.SetLayerMask(grabbableMask);

            Collider2D[] results = new Collider2D[1];
            int cnt = grabBox.Overlap(filter, results);
            if (cnt > 0 && results[0] != null && results[0].CompareTag("Grabbable"))
                c = Color.green;
        }
        catch {}

        Gizmos.color = c;
        Gizmos.DrawWireCube(grabBox.bounds.center, grabBox.bounds.size);
    }

    public string CurrentGrabbedName
    {
        get
        {
            if (carriedT != null)
                return carriedT.name;
            else
                return null;
        }
    }

    public bool IsCarryingSomething
    {
        get { return carriedT != null; }
    }
}
