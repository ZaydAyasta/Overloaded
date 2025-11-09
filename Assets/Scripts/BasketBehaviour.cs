using UnityEngine;

public class BasketBehaviour : GrabbableBehaviour
{
    [Header("Interacción")]
    public KeyCode useKey = KeyCode.E;

    [Header("Detección")]
    public Transform tip;                  // punto de detección (frente a la cesta cuando la llevas)
    public float detectRadius = 0.7f;
    public LayerMask clotheslineMask;      // capa del tendedero

    [Header("Sprites")]
    public Sprite fullSprite;
    public Sprite emptySprite;

    [Header("Estado")]
    [SerializeField] private bool isEmpty = false;  // comienza llena
    public bool IsEmpty => isEmpty;

    private Clothesline nearby;
    private bool nearInteractable = false;
    private SpriteRenderer sr;

    // Edge detection robusto
    private bool prevKeyHeld = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>(true);
        RefreshSprite();
    }

    void RefreshSprite()
    {
        if (sr == null) return;
        sr.sprite = isEmpty ? emptySprite : fullSprite;
    }

    public override bool WantsToBlockDrop()
        => (!isEmpty && (nearInteractable || (nearby != null && nearby.IsBusy)));

    public override string GetDropBlockHint()
        => (!isEmpty && (nearInteractable || (nearby != null && nearby.IsBusy))) ? "No puedes soltar mientras cuelgas la ropa." : null;

    public override void OnPickedUp(PlayerGrab player)
    {
        prevKeyHeld = false;
        nearby = null;
        nearInteractable = false;
    }

    public override void OnDropped(PlayerGrab player)
    {
        prevKeyHeld = false;
        nearby = null;
        nearInteractable = false;
    }

    public override void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput)
    {
        if (isEmpty) return; // ya no sirve

        // Detectar tendedero
        nearby = null; nearInteractable = false;
        if (tip != null)
        {
            var hits = Physics2D.OverlapCircleAll(tip.position, detectRadius, clotheslineMask);
            float best = float.MaxValue;
            foreach (var h in hits)
            {
                var c = h.GetComponent<Clothesline>() ?? h.GetComponentInParent<Clothesline>();
                if (c == null || c.IsCompleted) continue;
                float d = (tip.position - h.bounds.ClosestPoint(tip.position)).sqrMagnitude;
                if (d < best) { best = d; nearby = c; nearInteractable = true; }
            }
        }

        // Input
        bool keyHeld = Input.GetKey(useKey);
        bool justPressed = keyHeld && !prevKeyHeld;

        if (nearby != null && justPressed)
        {
            // Le pedimos al tendedero que ejecute el proceso (bloquea movimiento, humo, etc.)
            nearby.TryHang(player, this);
        }

        prevKeyHeld = keyHeld;
    }

    /// <summary>Lo llama el tendedero cuando la tarea termina correctamente.</summary>
    public void MarkEmptiedAndDisable()
    {
        isEmpty = true;
        RefreshSprite();

        // Deshabilitar agarrado/interacción
        var col = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>(true);
        if (col) col.enabled = false;

        // Si tienes un wrapper "Grabbable" además de este Behaviour, puedes deshabilitarlo también:
        var grab = GetComponent<Grabbable>();
        if (grab) grab.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (tip == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(tip.position, detectRadius);
    }
}
